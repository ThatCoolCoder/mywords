# MyWords

Customisable and non-dictating language learning app.

Current status: Aiming to achieve functionality with a passable appearance.

## Dev server

Run `npm install` before running for the first time.

Then just `npm run dev` (rollup is configured to run dotnet).

## Building for deployment

Uuh we haven't gotten that far yet. Probably just `npm build` & convince rollup to also run dotnet.

## Conventions

Order of imports:

- Svelte things or things from important libraries like routify
- Local code imports
- Local component imports, or other UI things such as actions

Currently there aren't any 3rd party code or component libraries so there isn't a place for them designated yet.