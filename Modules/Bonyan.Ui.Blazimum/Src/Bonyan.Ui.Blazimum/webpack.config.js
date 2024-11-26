// webpack.config.js
const path = require('path');
const TerserPlugin = require('terser-webpack-plugin');

module.exports = {
    // Configuration for UMD build
    umd: {
        entry: './Scripts/index.umd.js',          // UMD entry
        output: {
            path: path.resolve(__dirname, 'wwwroot/js'),
            filename: 'blazimum.umd.js',          // UMD output file
            library: 'Blazimum',
            libraryTarget: 'umd',
            libraryExport: 'default',
            globalObject: 'this',
        },
        module: {
            rules: [
                {
                    test: /\.js$/,
                    exclude: /node_modules/,
                    use: {
                        loader: 'babel-loader',
                        options: {
                            presets: ['@babel/preset-env'],
                        },
                    },
                },
            ],
        },
        optimization: {
            minimize: true,
            minimizer: [new TerserPlugin()],
        },
    },

    // Configuration for ESM build
    esm: {
        entry: './Scripts/index.esm.js',          // ESM entry
        output: {
            path: path.resolve(__dirname, 'wwwroot/js'),
            filename: 'blazimum.esm.js',          // ESM output file
            library: {
                type: 'module',
            },
        },
        module: {
            rules: [
                {
                    test: /\.js$/,
                    exclude: /node_modules/,
                    use: {
                        loader: 'babel-loader',
                        options: {
                            presets: ['@babel/preset-env'],
                        },
                    },
                },
            ],
        },
        experiments: {
            outputModule: true,
        },
        optimization: {
            minimize: true,
            minimizer: [new TerserPlugin()],
        },
    },
};
