## SharpZip

SharpZip is a Cobalt Strike compatible .Net assembly that facilitates data extraction. 

It compresses a specified folder to a zip file. 

If errors occurs while trying to read a file, for example: if the file is locked, SharpZip will retry 5 times (by default) and will ignore the file if all retries fail.

~~~
Sharp.exe <dir> <zip_file> [retryCount: default=5]

Example:
Sharp.exe C:\temp\ c:\archive.zip 10
~~~

