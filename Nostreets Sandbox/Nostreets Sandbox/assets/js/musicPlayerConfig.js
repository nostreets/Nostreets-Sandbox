//# sourceURL=musicPlayerConfig.js
(function (songs) {


    render();

    function render() {


        var maxSideNum = 24,
            maxRectangleNum = 24;

        // Defaults
        var Options = function () {
            this.height = 300;
            this.radius = 360;
            this.sideCount = 24;
            this.rotSpeed = -0.3;

            this.rectangleCount = 16;
            this.rectangleWidth = 80;
            this.vertMargin = 10;
            this.borderWidth = 3;

            this.color = 2;
            this.solidBG = true;
            this.rainbowMode = false;
            this.animateThroughSpectrum = true;
            this.fade = false;
        };

        // dat.gui setup
        var myOptions = new Options(),
            gui = new dat.GUI(),
            f1 = gui.addFolder("Prism Controls"),
            f2 = gui.addFolder("Rectangle Controls"),
            f3 = gui.addFolder("Color Controls"),
            mySideCount = f1.add(myOptions, "sideCount", 3, maxSideNum).step(1),
            myRadius = f1.add(myOptions, "radius", 30, 600).step(15),
            myHeight = f1.add(myOptions, "height", 50, 750).step(50),
            myRotSpeed = f1.add(myOptions, "rotSpeed", -1, 1).step(0.1),
            myRectangleCount = f2.add(myOptions, "rectangleCount", 3, maxRectangleNum).step(1),
            myRectangleWidth = f2.add(myOptions, "rectangleWidth", 1, 100).step(5),
            myVertMargin = f2.add(myOptions, "vertMargin", 0, 15).step(1),
            myBorderWidth = f2.add(myOptions, "borderWidth", 0, 15).step(1),
            myColor = f3.add(myOptions, "color", 0, 360).step(1),
            mySolidBG = f3.add(myOptions, "solidBG"),
            myRainbow = f3.add(myOptions, "rainbowMode"),
            myAnimateThroughSpectrum = f3.add(myOptions, "animateThroughSpectrum"),
            myFade = f3.add(myOptions, "fade");

        f2.open();

        var audio, analyser, audioctx, sourceNode, stream;

        var c = 0, // Used to change color over time
            activeSongBtn = null;

        var prism = document.querySelector(".prism"),
            sides = document.querySelectorAll(".side"),
            rectangleArray = [maxSideNum],
            lastTime = Date.now(),
            timeGap = 50,
            rotAmt = 0; // Starting rotation of prism in degrees

        var sectionsAveraged = [maxSideNum],
            countSinceLast = [maxSideNum];


        renderPrism();
        datGuiListeners();
        rotatePrism();

        //destroyAll();
        appendSongs();
        musicPlayerListeners();



        function renderPrism() {
            rectangleSetup();
            sideCountChange(myOptions.sideCount);
            radiusChange(myOptions.radius);
            heightChange(myOptions.height);
            rectangleCountChange(myOptions.rectangleCount);
            rectangleWidthChange(myOptions.rectangleWidth);
            vertMarginChange(myOptions.vertMargin);
            borderWidthChange(myOptions.borderWidthChange);
            colorChange(myOptions.color);
            soildGBChange(myOptions.solidBG);

        }

        function datGuiListeners() {
            mySideCount.onFinishChange(sideCountChange);
            myHeight.onFinishChange(heightChange);
            myRectangleCount.onFinishChange(rectangleCountChange);
            myRectangleWidth.onFinishChange(rectangleWidthChange);
            myVertMargin.onFinishChange(vertMarginChange);
            myBorderWidth.onFinishChange(borderWidthChange);
            myColor.onFinishChange(colorChange);
            mySolidBG.onFinishChange(soildGBChange);
            myRainbow.onFinishChange(goRainbowMode);
            myRadius.onFinishChange(radiusChange);
        }


        // Render prism functions
        function rectangleSetup() {
            for (var i = 0; i < maxSideNum; i++) {
                rectangleArray[i] = sides[i].querySelectorAll(".rectangle");
            }
        }

        function sideCountChange(newCount) {
            [].forEach.call(sides, function (elem, i) {
                if (i < myOptions.sideCount) {
                    // The circle is inscribed inside of the prism, so we can use this formula to calculate the side length
                    var sideLength =
                        2 * myOptions.radius * Math.tan(Math.PI / newCount);
                    prism.style.width = sideLength + "px";
                    prism.style.left = "calc(50% - " + sideLength / 2 + "px)";

                    sides[i].style.transform =
                        "rotateY(" +
                        i * (360 / newCount) +
                        "deg) translateZ(" +
                        myOptions.radius +
                        "px) rotateX(180deg)";
                    sides[i].classList.remove("hide");
                } else {
                    sides[i].classList.add("hide");
                }
            });
        }

        function radiusChange(newRadius) {
            sideCountChange(myOptions.sideCount);
        }

        function heightChange(newHeight) {
            prism.style.height = newHeight + "px";
            prism.style.top = "calc(50% - " + newHeight / 2 + "px)";
            rectangleCountChange(myOptions.rectangleCount);
        }

        function rectangleCountChange(newCount) {
            [].forEach.call(rectangleArray, function (side, i) {
                [].forEach.call(side, function (rect, i) {
                    if (i < myOptions.rectangleCount) {
                        rect.style.height =
                            (myOptions.height - myOptions.vertMargin) / newCount -
                            myOptions.vertMargin +
                            "px";
                        rect.classList.remove("hide");
                    } else {
                        rect.classList.add("hide");
                    }
                });
            });
        }

        function rectangleWidthChange(newWidth) {
            [].forEach.call(rectangleArray, function (side, i) {
                [].forEach.call(side, function (rect, i) {
                    rect.style.width = newWidth + "%";
                });
            });
        }

        function vertMarginChange(newMargin) {
            [].forEach.call(rectangleArray, function (side, i) {
                [].forEach.call(side, function (rect, i) {
                    rect.style.margin = newMargin + "px auto";
                });
            });
            rectangleCountChange(myOptions.rectangleCount);
        }

        function borderWidthChange(newWidth) {
            [].forEach.call(rectangleArray, function (side, i) {
                [].forEach.call(side, function (rect, i) {
                    rect.style.borderWidth = newWidth + "px";
                });
            });
        }

        function colorChange(value) {
            if (!myOptions.rainbowMode)
                [].forEach.call(sides, function (elem, i) {
                    sides[i].style.color =
                        "hsl(" +
                        value +
                        ", 55%, " +
                        (20 + i / myOptions.sideCount * 40) +
                        "%)";
                });
        }

        function soildGBChange(value) {
            if (value === true) prism.classList.add("solid");
            else prism.classList.remove("solid");
        }

        function goRainbowMode(value) {
            [].forEach.call(sides, function (elem, i) {
                if (value === true)
                    sides[i].style.color =
                        "hsl(" + 360 * (i / myOptions.sideCount) + ", 80%, 55%)";
                else colorChange(myOptions.color);
            });
        }

        function checkAnimateThroughSpectrum() {
            if (myOptions.animateThroughSpectrum)
                [].forEach.call(sides, function (elem, i) {
                    sides[i].style.color =
                        "hsl(" +
                        c +
                        ", 80%, " +
                        (20 + i / myOptions.sideCount * 40) +
                        "%)";
                });
            else if (myOptions.rainbowMode) goRainbowMode(true);
            else colorChange(myOptions.color);
        }




        // The music functions
        function musicPlayerListeners() {

            $('.playPauseButton').on('click', function (a, b) {
                var btn = $(this),
                    activeSongUrl = btn.data('music'),
                    previousSongUrl = (activeSongBtn) ? activeSongBtn.data('music') : '';

                if (activeSongUrl === previousSongUrl)
                    playPause(btn, (btn.text() === "▮▮") ? 'pause' : 'playSame');
                else
                    getSongFromUrl(window.location.origin + activeSongUrl, (obj) => {
                        stream = window.URL.createObjectURL(obj);
                        loadSong(stream, btn);
                    });

            });
        }

        function loadSong(stream, button) {

            toggleSongLinks(button);
            setupSong();

            audio.src = stream;

            playPause(button, 'playNew');
            updatePrism();
        }

        function setupSong() {
            // Stop the previous song if there is one
            if (audio && activeSongBtn)
                playPause(activeSongBtn, 'pause');

            audio = new Audio();
            audioctx = new AudioContext();
            analyser = audioctx.createAnalyser();
            analyser.smoothingTimeConstant = 0.75;
            analyser.fftSize = 512;

            audio.addEventListener("ended", songEnded, false);

            sourceNode = audioctx.createMediaElementSource(audio);
            sourceNode.connect(analyser);
            sourceNode.connect(audioctx.destination);
        }

        function songEnded() {
            playPause(activeSongBtn);
        }

        function playPause(button, action) {

            switch (action) {
                case 'playNew': {
                    if (activeSongBtn && activeSongBtn.text() === "▮▮") {
                        activeSongBtn.text("▶");
                        audio.pause();
                    }

                    button.text("▮▮");
                    activeSongBtn = button;
                    audio.play();
                    break;
                }

                case 'playSame': {
                    if (activeSongBtn.text() === "▶") {
                        activeSongBtn.text("▮▮");
                        audio.play();
                    }
                    break;
                }

                case 'pause': {
                    if (activeSongBtn.text() === "▮▮") {
                        activeSongBtn.text("▶");
                        audio.pause();
                    }
                    break;
                }

            }
        }


        // The drawing functions
        function drawSide(freqSequence, freqPercent) {
            // Get the number of rectangles based on the freqValue
            drawRectangles(
                freqSequence,
                Math.floor(freqPercent * myOptions.rectangleCount / 100)
            );
        }

        function drawRectangles(sideNum, numRectanglesShowing) {
            for (var i = 0; i < myOptions.rectangleCount; i++) {
                var cl = rectangleArray[sideNum][i].classList;
                if (i <= numRectanglesShowing) {
                    cl.remove("hide");
                    cl.remove("faded");
                } else {
                    if (!myOptions.fade) cl.add("hide");
                    else cl.add("faded");
                }
            }
        }

        function updatePrism() {
            var currTime = Date.now();

            var freqArray = new Uint8Array(analyser.frequencyBinCount);
            analyser.getByteTimeDomainData(freqArray);

            // Find the average of the values near to each other (grouping)
            var average = 0,
                count = 0,
                numPerSection = 256 / (myOptions.sideCount + 1),
                nextSection = numPerSection;

            for (var i = 0; i < freqArray.length; i++) {
                var v = freqArray[i];
                average += Math.abs(128 - v); // 128 is essentially 0
                count++;

                if (i > nextSection) {
                    var currentSection = Math.floor(i / numPerSection - 1);

                    sectionsAveraged[currentSection] += average / count;
                    countSinceLast[currentSection]++;

                    average = 0;
                    count = 0;
                    nextSection += numPerSection;
                }
            }

            // Find the average of the values since the last time checked per section (smoothing)
            if (currTime - lastTime > timeGap) {
                for (var i = 0; i < myOptions.sideCount; i++) {
                    drawSide(i, sectionsAveraged[i] / countSinceLast[i] * 2.75, c);
                    sectionsAveraged[i] = 0;
                    countSinceLast[i] = 0;
                }

                lastTime = currTime;
            }

            checkAnimateThroughSpectrum();

            c += 0.5;
            requestAnimationFrame(updatePrism);
        }


        //Custom Logic

        //Destroy Everything
        function destroyAll() {
            
            f3.remove(myFade);
            f3.remove(myAnimateThroughSpectrum);
            f3.remove(myRainbow);
            f3.remove(mySolidBG);
            f3.remove(myColor);

            f2.remove(myBorderWidth);
            f2.remove(myVertMargin);
            f2.remove(myRectangleWidth);
            f2.remove(myRectangleCount);

            f1.remove(myRotSpeed);
            f1.remove(myHeight);
            f1.remove(myRadius);
            f1.remove(mySideCount);

            gui.removeFolder(f3);
            gui.removeFolder(f2);
            gui.removeFolder(f1);

            gui.destroy();
        }

        // Rotate the prism
        function rotatePrism() {
            prism.style.transform = "rotateY(" + rotAmt + "deg)";
            rotAmt += 3 * myOptions.rotSpeed;
            if (rotAmt > 360 || rotAmt < -360) rotAmt = 0;
            requestAnimationFrame(rotatePrism);
        }

        // Render songs
        function appendSongs() {
            if (songs && songs.length) {

                for (var song of songs) {

                    var songRow = $($('#song-list').children()[0]).clone();
                    songRow.find('.songTitle').text(song.title);
                    songRow.find('.playPauseButton').data('music', song.musicPath);
                    songRow.find('.songImg').attr('src', song.imgPath);

                    //LINKS
                    songRow.find('.fa-spotify').parent().attr('href', song.spotify);
                    songRow.find('.fa-soundcloud').parent().attr('href', song.soundcloud);
                    songRow.find('.fa-youtube').parent().attr('href', song.youtube);


                    $('#song-list').append(songRow);
                }

                $('#song-list').children()[0].remove();
                $('#song-list').removeClass('hide');
            }
        }

        //Get Song From Url
        function getSongFromUrl(url, callback) {

            //VANILLA WAY
            //var xhr = new XMLHttpRequest();
            //xhr.onreadystatechange = () => {
            //    console.log(xhr);
            //    if (xhr.status === 200)
            //        callback(xhr.response);
            //};
            //xhr.open('GET', url);
            //xhr.responseType = 'blob';
            //xhr.send();

            $.ajax({
                url: url,
                cache: false,
                xhr: function () {// Seems like the only way to get access to the xhr object
                    var xhr = new XMLHttpRequest();
                    xhr.responseType = 'blob';
                    return xhr;
                },
                success: callback,
                error: (err) => {
                    console.log(err);
                }
            });
        }

        //Toggle The Song Links
        function toggleSongLinks(button) {

            if (activeSongBtn) {
                var activeLinks = activeSongBtn.parent().parent().find('.song-links');
                page.animate(activeLinks, 'fadeOut', () => {
                    activeLinks.addClass('hide');
                }, 'faster');
            }

            var links = button.parent().parent().find('.song-links');
            links.removeClass('hide');
            page.animate(links, 'slideInDown', null, 'faster');
        }

    }

})(songs);