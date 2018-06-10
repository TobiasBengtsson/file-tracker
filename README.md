# file-tracker
This simple tool can be used to keep track of files and remove duplicates.

## Use case

When you feel like you have multiple backups of every file on your computer(s)
but don't have any way of easily deleting the duplicates.

Let's say you have chosen to store all your files in one place (might be for
example a cloud storage provider). After moving most of your files you will
notice that you find lots of duplicates files lying around, that you have
already migrated to the new place and can safely delete. The problem is that
for each file you need to check if it is actually a duplicate or not, and
this might take an insane amount of time. The files might even have different
names making the process almost impossible to do manually..

Meet *file-tracker*. With file-tracker you can:
1) Index all your files  from your target storage solution into a simple text
   file containing the file names and hashes.
2) Using the file from 1), you can automatically delete any duplicates in any
   other folder on your computer or hard drive. If you want, you can also just
   list the duplicates to take manual action later.

## Usage

### Index files

Print file names and file hashes to STDOUT 
`file-tracker index /path/to/dir`

Print file names and file hashes to file
`file-tracker index /path/to/dir -o -path/to/output/file`
