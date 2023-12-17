// Equivalent of c# extension methods for dumb things missing in javascript

Array.prototype.delete = function(index) {
    var modified = this.concat([]);
    modified.splice(index, 1);
    return modified;
}