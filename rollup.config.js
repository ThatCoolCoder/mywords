import svelte from 'rollup-plugin-svelte';
import commonjs from '@rollup/plugin-commonjs';
import resolve from '@rollup/plugin-node-resolve';
import livereload from 'rollup-plugin-livereload';
import { terser } from 'rollup-plugin-terser';
import css from 'rollup-plugin-css-only';
import json from '@rollup/plugin-json';
import includePaths from 'rollup-plugin-includepaths';
import alias from '@rollup/plugin-alias';

const production = !process.env.ROLLUP_WATCH;

function serve() {
	let server;

	function toExit() {
		if (server) server.kill(0);
	}

	return {
		writeBundle() {
			if (server) return;
			server = require('child_process').spawn('dotnet', ['watch', 'run'], {
				stdio: ['inherit', 'inherit', 'inherit'],
				shell: true
			});

			process.on('SIGTERM', toExit);
			process.on('exit', toExit);
		}
	};
}

export default {
	input: 'svelte-app/main.js',
	output: {
		sourcemap: true,
		format: 'iife',
		name: 'app',
		file: 'wwwroot/build/bundle.js',
		inlineDynamicImports: true,
	},
	plugins: [
        json(),
		svelte({
			compilerOptions: {
				// enable run-time checks when not in production
				dev: !production
			}
		}),
		// we'll extract any component CSS out into
		// a separate file - better for performance
		css({ output: 'bundle.css' }),

		// If you have external dependencies installed from
		// npm, you'll most likely need these plugins. In
		// some cases you'll need additional configuration -
		// consult the documentation for details:
		// https://github.com/rollup/plugins/tree/master/packages/commonjs
		resolve({
			browser: true,
			dedupe: ['svelte', 'svelte/transition', 'svelte/internal']
		}),
		commonjs(),

		// In dev, run dotnet
		!production && serve(),

		// Watch the `public` directory and refresh the
		// browser on changes when not in production
		!production && livereload('wwwroot'),

		// If we're building for production (npm run build
		// instead of npm run dev), minify
		production && terser(),
		alias({
			resolve: ['.svelte', '.js'], //optional, by default this will just look for .js files or folders
			entries: [
				// Please keep this in sync with jsconfig.json
				{ find: 'services', replacement: './svelte-app/services' },
				{ find: 'pages', replacement: './svelte-app/pages' },
				{ find: 'shared', replacement: './svelte-app/shared' },
				{ find: 'data', replacement: './svelte-app/data' },
			]
		}),
		includePaths({ paths: ["./"] }),
	],
	watch: {
		clearScreen: false
	}
};
