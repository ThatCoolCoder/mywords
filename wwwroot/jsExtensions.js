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