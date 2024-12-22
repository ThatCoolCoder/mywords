export function isCorrect(answer, definition) {
    // Basic operating principle:
    // split the answer and definition by ,
    // check that all of the sections within the provided answer are present in the correct answer

    // todo: wtf is this regex
    let clean = x => x.replace(/[^\p{Letter}\p{Mark}\s']/gu, "").trim().toLowerCase();

    let remainingAnswerSections = answer.split(',').map(clean);
    if (remainingAnswerSections.length == 0) return false;

    for (let section of definition.split(',')) {
        let idx = remainingAnswerSections.indexOf(clean(section));
        if (idx >= 0) remainingAnswerSections.splice(idx, 1);
    }
    return remainingAnswerSections.length == 0;
}

export function generateHint(definition) {
    return definition
        .trim()
        .replace(/^[^\p{Letter}]+/gu, "") // remove non-letters from start of line
        .replace(/(?!^)[\p{Letter}]/gu, "_") // replace all other letters with _
}