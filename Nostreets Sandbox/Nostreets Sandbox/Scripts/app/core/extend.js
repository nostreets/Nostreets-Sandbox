Array.prototype.add = function (obj) {
    if (this.findIndex((a) => a.key === obj.key) !== -1) {
        this[this.findIndex((a) => a.key === obj.key)] = obj;
    }
    else { this.push(obj); }
}

Array.prototype.findByKey = function (key) {
    if (this.findIndex((a) => a.key === key) !== -1) {
        return this[this.findIndex((a) => a.key === key)];
    }
    else { return null; }
}

Number.prototype.toKey = function (keyAndValueArr) {
    if (!Array.isArray(arr)) { return this; }
    var key = null;

    for (let item of keyAndValueArr) {
        if (!item || !item.key || !item.value) { continue; };

        key = (this === item.value) ? item.key : this;
    }
    return key;
}

String.prototype.toValue = function (keyAndValueArr) {
    if (!Array.isArray(arr)) { return this; }
    var value = null;

    for (let item of keyAndValueArr) {
        if (!item || !item.key || !item.value) { continue; };

        value = (this === item.key) ? item.value : this;
    }
    return value;
}

//Promise.prototype.toObject = function () {
//    promise = this;

//    // Don't modify any promise that has been already modified.
//    if (promise.isResolved) { return promise };

//    // Set initial state
//    var isPending = true;
//    var isRejected = false;
//    var isFulfilled = false;

//    // Observe the promise, saving the fulfillment in a closure scope.
//    var result = promise.then(
//        function (v) {
//            isFulfilled = true;
//            isPending = false;
//            return v;
//        },
//        function (e) {

//            switch (e) {
//                default:
//                    isRejected = true;
//                    isPending = false;
//                    break;
//            }


//            //throw e;
//        }
//    );

//    result.isFulfilled = function () { return isFulfilled; };
//    result.isPending = function () { return isPending; };
//    result.isRejected = function () { return isRejected; };
//    return result;
//}

//Promise.prototype.getState = function() {
//    const pending = {};
//    let status, value;
//    Promise.race([ this, pending ]).then(
//        x => { status = 'fulfilled'; value = x; },
//        x => { status = 'rejected'; value = x; }
//    );
//   // process._tickCallback();  // run microtasks right now
//    if( value === pending )
//        return { status: 'pending' };
//    return { status, value };
//};

//Promise.prototype.repeatUntilSuccessful = function (time, maxLoops, callback = null, currentIndex = 0) {

//    promise = this;


//    var delay = function (time, val) {
//        return new Promise(function (resolve) {
//            setTimeout(function () {
//                resolve(val);
//            }, time);
//        });
//    }


//    if (promise.getState() === '') {
//        return delay(1000).then(() => promise.repeatUntilSuccessful(time, maxLoops, currentIndex));
//    }


//    else //(promise.toObject().isPending() === false)
//    {
//        return promise.then(

//            (data) => { if (promise.toObject().isFulfilled() === true || currentIndex >= maxLoops) { if (callback !== null) { callback(data); } return; } },

//            (data) => {
//                if (currentIndex >= maxLoops) { return; }
//                if (promise.toObject().isRejected() === true) {

//                    return delay(time, data).then(
//                        () => promise.repeatUntilSuccessful(time, maxLoops, currentIndex++),
//                        () => promise.repeatUntilSuccessful(time, maxLoops, currentIndex++));
//                }
//            }
//        );
//    }
//} 