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