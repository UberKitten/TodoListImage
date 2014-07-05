TodoListImage
============

I use the todo.txt philosophy to keep track of tasks. I keep it in my Dropbox, but I wanted to have a better way to glance at the tasks on all my computers by putting it on the Desktop. I tried things like Rainlander but was frustrated at how I'd have to configure it on every computer, keep the configuration in sync, etc. Plus most of the solutions wouldn't work on Windows 8.

I realized all I needed was an image with a list of the tasks, synced by Dropbox, and used by every computer as its background. So I created this little service: run it on a server/always-on PC, it monitors your todo.txt and when it changes it creates two background PNGs in a folder you specify. Then on your computers with > Win Vista, point the Background chooser to the folder, choose the two PNGs, set it to "Center" and "change every 10 seconds." Tada. It just works.