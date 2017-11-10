# keylogger
Simple C# keylogger with screenshot support, uploading gathered data to online file storages and sending mail with links.

## Purpose of this project
This demonstrations just tries to show simple architecture for remote PC monitoring. Is's not designated as ready-to-use program. I just wanted to write something like that as a practice.

## Features:
* Keyboard tracking
* Screen snapshots
  * Possibility to set set of processes and screen snapshots will be made only if windows of these processes are active
* Uploading gathered data to online file storage services
* Sending mail with links to gathered data

## What's missing:
* Any attempts to hide this program (except hiding of the window)
  * Its process is called Keylogger :) 
* Any dealing with issues such as:
  * Gathered files are too big
  * Uploading or sending mails stops working
  * Other exceptions which might occur
* Any settings for autostart of the program
* Any way to control keylogger remotely and update it

## How it works
The application is WPF application which instanly after its launch creates a thread with keylogger and closes the WPF application. Therefore no GUI is displayed and only process remains. You can find it in task manager as *Keylogger* process. The sole main loop of the keylogger is in [Controller.cs](src/Keylogger/Controller.cs). In this file is also settings for mail services - usernames, passwords etc.

As we are getting some information from the system - pressed keys, screen content and active window, these access is wrapped up in [DataSource.cs](src/Keylogger/DataSource.cs)

Gathered data are continuously stored in files in *TEMP* directory of the system. The path of the directory is something like that: *C:\Users\\<\<user\>\>\AppData\Local\Temp\KeyloggerTemp*

From time to time the controlles performs a check of gathered data size and if it reaches set threshold it zips *KeyloggerTemp* directory and uploads it to all defined online file storages. For demonstration there is just [ExpireBox.cs](src/Keylogger/Upload/ExpireBox.cs) which aims to https://expirebox.com/. After upload, all gathered data are deleted from *KeyloggerTemp* directory and mail with links to uploaded file is sent to defined email. You can of course use gmail, but here I used free public service https://www.mailjet.com/. Code for email is in [MailJet.cs](src/Keylogger/Mail/MailJet.cs)


