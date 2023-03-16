# Database model

user

* phone_no string max 15 chars
* name     string max 15 chars
* age      integer 18-99
* sex      string K/M
* bio      string max 400 chars
* hidden   bool
* lat      decimal
* lon      decimal
* display_sex string max 2 chars
* display_min_age integer 0-100
* display_max_age integer 0-100, > display_min_age
* display_max_range integer 0-100

sms_code

* phone_no string max 15 chars
* code     string 6 chars
* is_used  bool
* sent_at  timestamp

photo

* user      user
* ordinal   integer
* file_path string max 255 chars

like

* liked_by   user
* liked_who  user
* created_at timestamp

match

* user1      user
* user2      user
* created_at timestamp

chat

* send_from  user
* send_to    user
* message    string max 255 chars
* created_at timestamp
