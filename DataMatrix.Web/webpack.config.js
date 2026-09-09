var path = require('path');
const VueLoaderPlugin = require('vue-loader/lib/plugin');
const TerserPlugin = require('terser-webpack-plugin');
const MiniCss = require('mini-css-extract-plugin');

module.exports = {
    entry: {
        'styles': './wwwroot/styles/site.scss',
        'components': './wwwroot/components/components.scss',

        'home': './wwwroot/scripts/view-models/home/home.js'
    },
    output: {
        path: path.resolve(__dirname, 'wwwroot/scripts/dist'),
        publicPath: '/scripts/dist/',
        filename: process.env.npm_lifecycle_event === 'build' ? '[name].js' : '[name].[contenthash].js'
    },
    module: {
        rules: [
            {
                test: /\.vue$/,
                loader: 'vue-loader'
            },
            {
                test: /\.css$/,
                use: [
                    'vue-style-loader',
                    'css-loader'
                ]
            },
            {
                test: /\.scss$/,
                use: [
                    'vue-style-loader',
                    'css-loader',
                    'sass-loader'
                ]
            },
            {
                test: /\.js$/,
                exclude: /(node_modules|bower_components)/,
                use: {
                    loader: 'babel-loader',
                    options: {
                        presets: ['@babel/preset-env'],
                        plugins: ['@babel/plugin-transform-runtime']
                    }
                }
            }
        ]
    },
    plugins: [
        new VueLoaderPlugin(),
        new MiniCss({
            filename: 'style.css'
        })
    ],
    resolve: {
        alias: {
            'vue$': 'vue/dist/vue.esm.js'
        },
        extensions: ['.js', '.vue', '.json']
    },
    optimization: {
        minimizer: [
            new TerserPlugin({
                extractComments: false
            })
        ]
    }
}