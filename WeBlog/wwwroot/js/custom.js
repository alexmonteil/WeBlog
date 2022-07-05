let index = 0;

function AddTag() {
    let tagEntry = document.getElementById("TagEntry");

    let newOption = new Option(tagEntry.value, tagEntry.value);
    document.getElementById("TagList").options[index++] = newOption;

    tagEntry.value = "";

    return true;
}

function DeleteTag() {

    let tagCount = 1;
    while (tagCount > 0) {

        let tagList = document.getElementById("TagList");
        let selectedIndex = tagList.selectedIndex;

        if (selectedIndex >= 0) {

            tagList.options[selectedIndex] = null;
            --tagCount;

        } else {

            tagCount = 0;
        }

        index--;
    }
}

// Takes a selector for a select Tag and selects all options
function SelectAllOptions(selector) {

    console.log("SelectAllOptions running");

    let list = document.querySelector(selector);
    let numberOfOptions = list.options.length;

    for (let i = 0; i < numberOfOptions; i++) {
        list.options[i].selected = true;
    }

    console.log(list.value);
}

document.querySelector("form").addEventListener('submit', () => {

    return SelectAllOptions("#TagList");
});