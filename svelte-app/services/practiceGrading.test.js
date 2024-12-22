import * as practiceGrading from './practiceGrading';


test('Marks direct answer', () => {
    expect(practiceGrading.isCorrect("dog", "dog")).toBe(true);
});
test('Direct empty answer is correct', () => {
    expect(practiceGrading.isCorrect("", "")).toBe(true);
});
test('capitals dont matter', () => {
    expect(practiceGrading.isCorrect("Dog", "dog")).toBe(true);
});
test('Leading trailing spaces and numbers dont matter ', () => {
    expect(practiceGrading.isCorrect("  Dog2  ", "dog")).toBe(true);
    expect(practiceGrading.isCorrect("Dog", " dog2  ")).toBe(true);
});
test('Simple incorrect answer', () => {
    expect(practiceGrading.isCorrect("Dog", "cat")).toBe(false);
});
test('Spaces in words arent ok', () => {
    expect(practiceGrading.isCorrect("c at", "cat")).toBe(false);
});

test('2 possibilities 1st present', () => {
    expect(practiceGrading.isCorrect("dog", "cat, dog")).toBe(true);
});
test('2 possibilities 2nd present', () => {
    expect(practiceGrading.isCorrect("cat", "cat, dog")).toBe(true);
});
test('2 possibilities both present reversed order', () => {
    expect(practiceGrading.isCorrect("dog, cat", "cat, dog")).toBe(true);
});
test('2 possibilities both present reversed order, mismatched case and punc', () => {
    expect(practiceGrading.isCorrect("dog, cAt!", "cat, dog")).toBe(true);
});
test('2 possibilities both present correct order', () => {
    expect(practiceGrading.isCorrect("cat, dog", "cat, dog")).toBe(true);
});
test('2 possibilities neither given', () => {
    expect(practiceGrading.isCorrect("big", "cat, dog")).toBe(false);
});
test('2 possibilities nothing given', () => {
    expect(practiceGrading.isCorrect("", "cat, dog")).toBe(false);
});
test('2 possibilities nothing given', () => {
    expect(practiceGrading.isCorrect("", "cat, dog")).toBe(false);
});


test('formatting basic', () => {
    expect(practiceGrading.generateHint("hello")).toBe("h____");
});
test('nonletters ignored at start', () => {
    expect(practiceGrading.generateHint(" hello")).toBe("h____");
    expect(practiceGrading.generateHint(",hello!")).toBe("h____!");
});
test('punc shown as punc', () => {
    expect(practiceGrading.generateHint("hello how are you")).toBe("h____ ___ ___ ___");
    expect(practiceGrading.generateHint("hello!dog")).toBe("h____!___");
});