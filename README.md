# relink
This is a quick tool that goes through a NTFS directory (may work on other file systems) and replaces all symbolic links with the files that they point to.  I used it to solve a very specific problem with a migration tool - it may or may not be useful to anyone else.

The behavior is:
* For Each directory
  - For Each File System Entry, is it a link?
  - IF so, do a recursive copy *with security attributes*
  - Next
* Next
