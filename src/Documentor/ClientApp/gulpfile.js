const gulp = require('gulp');
const del = require('del');
const imagemin = require('gulp-imagemin');
const changed = require('gulp-changed');
const concat = require('gulp-concat');
const merge = require('merge-stream');
const uglify = require('gulp-uglify');
const cleanCSS = require('gulp-clean-css');

function clean() {
    return del(['../wwwroot/assets/**', '!../wwwroot/assets'], { force: true });
}

function images() {
    return gulp
        .src('./static/images/**/*')
        .pipe(changed('../wwwroot/assets/images'))
        .pipe(
            imagemin([
                imagemin.gifsicle({ interlaced: true }),
                imagemin.mozjpeg({ quality: 75, progressive: true }),
                imagemin.optipng({ optimizationLevel: 5 }),
                imagemin.svgo({
                    plugins: [
                        {
                            removeViewBox: false,
                            collapseGroups: true
                        }
                    ]
                })
            ])
        )
        .pipe(gulp.dest('../wwwroot/assets/images'));
}

function fonts() {
    return gulp
        .src('./static/fonts/**/*')
        .pipe(gulp.dest('../wwwroot/assets/fonts'));
}

function cssFonts() {
    return gulp.src([
        './static/css/fonts/*.css'
    ])
        .pipe(concat('fonts.min.css'))
        .pipe(cleanCSS())
        .pipe(gulp.dest('../wwwroot/assets/css/fonts'));

}

function cssVendors() {
    var vendors = gulp.src([
        './static/css/vendors/bootstrap.min.css',
        './static/css/vendors/scrollbar.css',
        './static/css/vendors/toastr.min.css',
        './static/css/vendors/prism.css'
    ])
        .pipe(concat('vendors.min.css'))
        .pipe(cleanCSS())
        .pipe(gulp.dest('../wwwroot/assets/css/vendors'));
    var simplemde = gulp.src([
        './static/css/vendors/simplemde.min.css',
    ])
        .pipe(concat('simplemde.min.css'))
        .pipe(cleanCSS())
        .pipe(gulp.dest('../wwwroot/assets/css/vendors'));
    var jstree = gulp.src([
        './static/css/vendors/jstree.css',
    ])
        .pipe(concat('jstree.min.css'))
        .pipe(cleanCSS())
        .pipe(gulp.dest('../wwwroot/assets/css/vendors'));

    return merge(vendors, simplemde, jstree);
}

function cssApp() {
    const app = gulp.src([
        './static/css/app/app-notifications.css',
        './static/css/app/app.css'
    ])
        .pipe(concat('app.min.css'))
        .pipe(cleanCSS())
        .pipe(gulp.dest('../wwwroot/assets/css/app'));
    const editor = gulp
        .src('./static/css/app/app-editor.css')
        .pipe(concat('app-editor.min.css'))
        .pipe(cleanCSS())
        .pipe(gulp.dest('../wwwroot/assets/css/app'));
    const jstree = gulp
        .src('./static/css/app/app-jstree.css')
        .pipe(concat('app-jstree.min.css'))
        .pipe(cleanCSS())
        .pipe(gulp.dest('../wwwroot/assets/css/app'));

    return merge(app, editor, jstree);
}

function jsVendors() {
    const vendors = gulp.src([
        './static/js/jquery/*.js',
        './static/js/popper/*.js',
        './static/js/bootstrap/*.js',
        './static/js/jquery.scrollbar/*.js',
        './static/js/toastr/*.js',
        './static/js/prism/*.js',        
    ])
        .pipe(concat('vendors.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest('../wwwroot/assets/js/vendors'));
    const jstree = gulp.src([
        './static/js/jstree/*.js',
    ])
        .pipe(concat('jstree.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest('../wwwroot/assets/js/vendors'));    
    const simplemde = gulp.src('./static/js/simplemde/*.js')
        .pipe(concat('simplemde.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest('../wwwroot/assets/js/vendors'));
    
    return merge(vendors, jstree, simplemde);
}

function jsApp() {
    const app = gulp.src([
        './src/app/app-notifications.js',
        './src/app/app.js'
    ])
        .pipe(concat('app.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest('../wwwroot/assets/js/app'));
    const pages = gulp.src([
        './src/app/app-pages.js'
    ])
        .pipe(concat('app-pages.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest('../wwwroot/assets/js/app'));
    const editor = gulp.src([
        './src/app/app-editor.js'
    ])
        .pipe(concat('app-editor.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest('../wwwroot/assets/js/app'));
    const users = gulp.src([
        './src/app/app-users.js'
    ])
        .pipe(concat('app-users.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest('../wwwroot/assets/js/app'));

    return merge(app, pages, editor, users);
}

const css = gulp.parallel(cssFonts, cssVendors, cssApp);
const js = gulp.parallel(jsVendors, jsApp);
const build = gulp.series(clean, gulp.parallel(images, fonts, css, js));

exports.clean = clean;
exports.images = images;
exports.fonts = fonts;
exports.css = css;
exports.js = js;
exports.build = build;