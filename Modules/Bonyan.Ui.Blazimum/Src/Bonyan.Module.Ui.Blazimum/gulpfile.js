// gulpfile.js
const gulp = require('gulp');
const sass = require('gulp-sass')(require('sass'));
const cleanCSS = require('gulp-clean-css');
const rtlcss = require('gulp-rtlcss');
const rename = require('gulp-rename');
const webpack = require('webpack');
const webpackStream = require('webpack-stream');
const webpackConfig = require('./webpack.config.js');

// Paths
const paths = {
    scss: './Styles/blazimum.scss',     // SCSS source file
    cssOutput: './wwwroot/css',         // CSS output folder
    jsOutput: './wwwroot/js'            // JavaScript output folder
};

// Compile SCSS, minify, and generate RTL version
function compileSCSS() {
    return gulp.src(paths.scss)
        .pipe(sass().on('error', sass.logError))
        .pipe(cleanCSS())                   // Minify CSS
        .pipe(gulp.dest(paths.cssOutput))   // Output LTR CSS as blazimum.css
        .pipe(rtlcss())                     // Convert to RTL
        .pipe(rename({ suffix: '-rtl' }))   // Rename to blazimum-rtl.css
        .pipe(gulp.dest(paths.cssOutput));  // Output RTL CSS
}

// Bundle JavaScript with Webpack UMD configuration
function bundleUMD() {
    return webpackStream(webpackConfig.umd, webpack)
        .pipe(rename('blazimum.umd.js'))    // Final output name for UMD unminified
        .pipe(gulp.dest(paths.jsOutput));
}

// Bundle JavaScript with Webpack ESM configuration
function bundleESM() {
    return webpackStream(webpackConfig.esm, webpack)
        .pipe(rename('blazimum.esm.js'))    // Final output name for ESM unminified
        .pipe(gulp.dest(paths.jsOutput));
}

// Watch for changes in SCSS and JavaScript files
function watchFiles() {
    gulp.watch(paths.scss, compileSCSS);
    gulp.watch('./Scripts/**/*.js', gulp.series(bundleUMD, bundleESM));
}

// Define Gulp tasks for default, build, and watch
exports.default = gulp.series(compileSCSS, bundleUMD, bundleESM, watchFiles);
exports.build = gulp.series(compileSCSS, bundleUMD, bundleESM);
exports.watch = watchFiles;
