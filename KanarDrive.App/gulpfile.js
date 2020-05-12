"use strict";

const gulp = require('gulp');
const concat = require('gulp-concat');
const minify = require('gulp-minify');
const cleanCss = require('gulp-clean-css');
const clean = require('gulp-clean');

const commonJsFiles = [
    'ClientSideFiles/js/font-awesome.js',
    'ClientSideFiles/js/clock.js',
    'node_modules/jquery/dist/jquery.min.js',
    'node_modules/popper.js/dist/umd/popper.min.js',
    'node_modules/bootstrap/dist/js/bootstrap.min.js',
    'node_modules/bootstrap-fileinput/js/fileinput.min.js',
    'node_modules/bootstrap-fileinput/js/locales/pl.js',
    'node_modules/bootstrap-fileinput/js/plugins/piexif.min.js',
    'node_modules/bootstrap-fileinput/themes/fas/theme.min.js'
];

const specializedJsFiles = [
    'ClientSideFiles/js/cloud.js'
];

const cssFiles = [
    "node_modules/bootstrap/dist/css/bootstrap.min.css",
    'node_modules/bootstrap-fileinput/css/fileinput.min.css'
];

const images = [
    "node_modules/bootstrap-fileinput/img/loading.gif",
    "node_modules/bootstrap-fileinput/img/loading-sm.gif"
];

const outputPath = 'wwwroot';

function commonJavascript(cb) {
    return gulp.src(commonJsFiles)
        .pipe(concat('bundle.js'))
        .pipe(minify())
        .pipe(gulp.dest(outputPath));
}

function specializedJavaScript(cb) {
    return gulp.src(specializedJsFiles)
        .pipe(minify())
        .pipe(gulp.dest(outputPath + '/js'))
}

function css(cb) {
    return gulp.src(cssFiles)
        .pipe(concat('bundle-min.css'))
        .pipe(cleanCss())
        .pipe(gulp.dest(outputPath));
}

function removeBundleJs(cb) {
    return gulp.src(outputPath+'/bundle.js', {read: false})
        .pipe(clean());
}
function removePreMinifiedCloudScripts(cb) {
    return gulp.src(outputPath+'/js/cloud.js', {read: false})
        .pipe(clean());
}
function image(cb) {
    return gulp.src(images)
        .pipe(gulp.dest(outputPath + '/img'));
}

exports.default = gulp.parallel(gulp.series(gulp.parallel(commonJavascript, specializedJavaScript), gulp.series(removeBundleJs, removePreMinifiedCloudScripts)), css, image);