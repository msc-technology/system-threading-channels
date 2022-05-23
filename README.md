# ChannelsProto

Sample usage of package System.Threading.Channels.

## What is it?

Implements a really basic web crawler that takes one or more URLs on the command line, and crawls through the pages on each host, saving all responses to a zip file.
URLs and hrefs in recovered pages are used to find more pages on the same host.
Three implementations are provided: one fully synchronous scheduling its work explicitly; one async with Task, explicitly aggregating data from the various crawling tasks;
and the final implementation using channels to aggregate all results and push them to a consumer.

## How is System.Threading.Channels used?

A crawler in the application is a class navigating pages at its own pace on a specific host.
Each crawler is rate limited to a request every fifteen seconds, as a real crawler should be limited to avoid being flagged for abuse.
While a single crawler is waiting to work on a site however, other crawlers can send requests to different sites.
All crawlers pass then all these pages to a file storage implemented through a zip file.

Contrary to the other solutions, the channel based solution does not need to schedule work explicitly, and leaves the code better partitioned between each functionality.
