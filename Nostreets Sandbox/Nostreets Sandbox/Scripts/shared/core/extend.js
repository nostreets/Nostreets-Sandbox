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

Array.prototype.any = function (func) {
    if (!func instanceof Function)
        return false;
    else {
        for (let item of this) {
            if (func(item) === true)
                return true;
        }

        return false;
    }
}

Array.prototype.in = function (value) {
    for (let item of this) {
        if (item === value)
            return true;
    }
    return false;
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


String.prototype.safeName = function () {
    return this.replace(/[!\"#$%&'\(\)\*\+,\.\/:;<=>\?\@\[\\\]\^`\{\|\}~\s+]/g, '');
}



String.prototype.replaceAt = function (index, replacement, replacementLength) {
    return this.substr(0, index) + replacement + this.substr(index + replacementLength || index + replacement.length);
}


  