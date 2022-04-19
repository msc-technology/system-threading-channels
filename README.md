# ChannelsProto

Sample usage of package System.Threading.Channels.

## What is it?

Implements a really basic web crawler that takes one or more URLs on the command line, and crawls through the pages on each host, saving all responses to a zip file.
URLs and hrefs in recovered pages are used to find more pages on the same host.

## How is System.Threading.Channels used?

A crawler in the application is a class navigating pages at its own pace on a specific host.
Each crawler is rate limited to a request every two seconds, as a real crawler should be limited to avoid being flagged for abuse.
While a single crawler is waiting to work on a site however, other crawlers can send requests to different sites.
All these requests are done in their own thread, and all responses are funneled through a Channel instance to the single thread that is responsible for writing to disk.

The use of a Channel avoids contention on the file resource, and allows multi tasking without having to write an explicit scheduling logic.

## Remarks

System.Threading.Channels is more suited to be used with async/await, but the current version tries to enforce synchronous execution on the threads dedicated to the file writer and each crawler.
This is in order to highlight how Channels handle concurrent thread access.

A branch or subproject will be dedicated to a fully async implementation that runs on the thread pool, rather than on explicitly managed threads.
