A simple client to measure the time taken to download a specific resource.

Usage: DownloadTest.exe URL waitTime {logFilePath}
       URL: Valid URL with no authentication. Should be of a reasonably large size (e.g. an image).
       waitTime: time in seconds between gets.  Use zero to get one time and then exit.
       logFilePath (optional): file for results. If not specified then results are written to console.

