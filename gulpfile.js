require('dotenv').config(); //.env file

var gulp = require('gulp'),
    //nuget
    es = require('event-stream'),
    watch = require('gulp-watch'),
    exec = require('child_process').exec;

// #region NuGet
var nuget = {
    paths: ['src/core/bin/Release/*.nupkg', 'src/modules/**/bin/Release/*.nupkg'],
    exe: process.env.NUGET_EXE, //path to your NuGet cmd, i.e. "C:\Program Files\nuget\nuget.exe"
    host: 'https://pkg.websolute.it/api/v2/package/',
    token: process.env.NUGET_TOKEN
}

pushPkg = function (es) {
    return es.map(function (file, cb) {

        var path = file.path;
        var _file = path.split('\\').pop();
        var _dots = _file.split('.'); _dots = _dots.slice(0, _dots.length - 1);

        var pkg = _dots.slice(0, _dots.length - 3).join('.'); 
        var version = _dots.reverse().slice(0, 3).reverse().join('.');

        var del = nuget.exe + ' delete ' + pkg + ' ' + version + ' -Source ' + nuget.host + ' -ApiKey ' + nuget.token;
        var push = nuget.exe + ' push ' + path + ' -Source ' + nuget.host + ' -ApiKey ' + nuget.token;

        exec(del, function (err, stdout, stderr) {
            exec(push, function (err, stdout, stderr) {
                console.log(err, stdout, stderr);
            });        
        });

        return cb();
    });
};

gulp.task('nuget', function () {       
    return gulp
        .src(nuget.paths)
        .pipe(pushPkg(es));    
});

// #endregion

gulp.task('watch', function () {
    watch(nuget.paths)
    .pipe(pushPkg(es));
});

gulp.task('default', gulp.series('watch'));