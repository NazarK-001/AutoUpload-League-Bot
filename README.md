# AutoUpload Bot

A Discord bot that automatically collects specific file formats from chat, processes them, and sends extracted data to a Strapi server. Displays results in chat using embeds.
Processes the files in batches and orders the response.

## Features
- The bot passively monitors the chat and reacts when relevant files are posted.
- Can process any amount of files posted at once. Responds with a group of embeds displaying details about every file. The response embeds are ordered starting from older files to newer.
- Extracts the data from files. Validates it and performs calculations. Checks it with the data that's already available on server.
- Forms the data into json requests. Uses Get, Post, Put and RESTful APIs to interact with Strapi server.
