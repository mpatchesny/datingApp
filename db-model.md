# Database model

account

* phone_no string max 15 chars
* name     string max 15 chars
* age      integer 18-99
* sex      string K/M
* bio      string max 400 chars
* hidden   bool
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

* account   account
* is_main   bool
* file_path string max 255 chars

like

* liked_by   account
* liked_who  account
* created_at timestamp

pair

* account1   account
* account2   account
* created_at timestamp

chat

* send_from  account
* send_to    account
* message    string 100 chars
* created_at timestamp
