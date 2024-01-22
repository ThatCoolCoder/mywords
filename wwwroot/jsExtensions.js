// Equivalent of c# extension methods for dumb things missing in javascript

Array.prototype.deleteIdx = function(index) {
    var modified = this.concat([]);
    modified.splice(index, 1);
    return modified;
}

Array.prototype.deleteItem = function(item) {
    return this.filter(x => x != item);
}

Array.prototype.pushed = function(item) {
    var modified = this.concat([item]);
    return modified;
}

// Can't put these in prototype because svelte seems to think they're props
Object.appended = function(obj, key, item) {
    var toAdd = {};
    toAdd[key] = item;
    return Object.assign(toAdd, obj);
}

Object.deleted = function(obj, key) {
    var toModify = Object.assign({}, obj);
    delete toModify[key];
    return toModify;
}

String.prototype.compareTo = function(other) {
    // used in sorting such as how you would do with integers arr.sort((a, b) => a - b);
    // (required if trying to sort by a string sub-property)
    if (this == other) return 0;
    var arr = [this, other];
    arr.sort();
    return (arr[0] == this) ? -1 : 1;
}