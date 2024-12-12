export function isCorrect(answer, definition) {
    // todo: no clue what this does
    let clean = x => x.replace(/[^\p{Letter}\p{Mark}\s']/gu, "").trim().toLowerCase();

    let remainingAnswerSections = answer.split(',').map(clean);
    if (remainingAnswerSections.length == 0) return false;

    for (let section of definition.split(',')) {
        let idx = remainingAnswerSections.indexOf(clean(section));
        if (idx >= 0) remainingAnswerSections.splice(idx, 1);
    }
    return remainingAnswerSections.length == 0;
}