I'm still casually interested in improving the Windows file search experience. Not sure a WinForms app is the answer, but as usual this is mainly an exercise to stimulate thinking. I poked around with a [SqlCe-based file search client](https://github.com/adamosoftware/WinSearch2) a while back. I felt like taking another swing at this without the database dependency. Here's what this looks like so far:

![img](https://adamosoftware.blob.core.windows.net:443/images/WinSearch3.png)

The work is done in the [FileSearch](https://github.com/adamosoftware/WinSearch3/blob/master/WinSearch3.Library/FileSearch.cs) class.
