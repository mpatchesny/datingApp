using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Application.PhotoManagement;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class AddPhotoHandler : ICommandHandler<AddPhoto>
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPhotoService _photoService;
    public AddPhotoHandler(IPhotoRepository photoRepository, IUserRepository userRepository, IPhotoService photoService)
    {
        _photoRepository = photoRepository;
        _userRepository = userRepository;
        _photoService = photoService;
    }

    public async Task HandleAsync(AddPhoto command)
    {
        // https://stackoverflow.com/questions/51300523/how-to-use-span-in-convert-tryfrombase64string
        byte[] bytes = new byte[((command.Base64Bytes.Length * 3) + 3) / 4 -
            (command.Base64Bytes.Length > 0 && command.Base64Bytes[command.Base64Bytes.Length - 1] == '=' ?
                command.Base64Bytes.Length > 1 && command.Base64Bytes[command.Base64Bytes.Length - 2] == '=' ?
                    2 : 1 : 0)];

        if (!Convert.TryFromBase64String(command.Base64Bytes, bytes, out int bytesWritten))
        {
            throw new FailToConvertBase64StringToArrayOfBytes();
        }

        // FIXME: move min, maxPhotoSize to app settings
        // // 2MB
        // const int maxPhotoSizeBytes = 2*1024*1024;
        // const int maxPhotoSizeMB = (int) maxPhotoSizeBytes / (1024*1024);
        // const int minPhotoSizeBytes = 100*1024;
        // const int minPhotoSizeKB = (int) minPhotoSizeBytes / 1024;
        // if (bytes.Count() > maxPhotoSizeBytes)
        // {
        //     throw new InvalidPhotoSizeException(minPhotoSizeKB, maxPhotoSizeMB);
        // }
        // if (bytes.Count() < minPhotoSizeBytes)
        // {
        //     throw new InvalidPhotoSizeException(minPhotoSizeKB, maxPhotoSizeMB);
        // }

        var user = await _userRepository.GetByIdAsync(command.UserId);
        if (user == null)
        {
            throw new UserNotExistsException(command.UserId);
        }

        if (user.Photos.Count() >= 6)
        {
            throw new UserPhotoLimitException();
        }

        int oridinal = user.Photos.Count();
        var photoPath = _photoService.SavePhoto(bytes);
        var photo = new Photo(command.PhotoId, command.UserId, photoPath, oridinal);
        await _photoRepository.AddAsync(photo);
    }
}