Array.prototype.add = function (obj)
{
    if (this.findIndex((a) => a.key === obj.key) !== -1)
    {
        this[this.findIndex((a) => a.key === obj.key)] = obj;
    }
    else { this.push(obj); }
}

Array.prototype.findByKey = function (key)
{
    if (this.findIndex((a) => a.key === key) !== -1)
    {
        return this[this.findIndex((a) => a.key === key)];
    }
    else { return null; }
}

Number.prototype.toKey = function (keyAndValueArr)
{
    if (!Array.isArray(arr)) { return this; }
    var key = null;

    for (let item of keyAndValueArr)
    {
        if (!item || !item.key || !item.value) { continue; };

        key = (this === item.value) ? item.key : this;
    }
    return key;
}

String.prototype.toValue = function (keyAndValueArr)
{
    if (!Array.isArray(arr)) { return this; }
    var value = null;

    for (let item of keyAndValueArr)
    {
        if (!item || !item.key || !item.value) { continue; };

        value = (this === item.key) ? item.value : this;
    }
    return value;
}

Promise.prototype.toObject = function (promise) {
    if (!promise) { promise = this; }

    // Don't modify any promise that has been already modified.
    if (promise.isResolved) return promise;

    // Set initial state
    var isPending = true;
    var isRejected = false;
    var isFulfilled = false;

    // Observe the promise, saving the fulfillment in a closure scope.
    var result = promise.then(
        function(v) {
            isFulfilled = true;
            isPending = false;
            return v; 
        }, 
        function(e) {
            isRejected = true;
            isPending = false;
            throw e; 
        }
    );

    result.isFulfilled = function() { return isFulfilled; };
    result.isPending = function() { return isPending; };
    result.isRejected = function() { return isRejected; };
    return result;
}
