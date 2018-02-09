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

