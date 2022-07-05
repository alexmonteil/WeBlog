let index = 0;

// Adds the tag entered in the text box to the select list options
function AddTag() {
    let tagEntry = document.getElementById("TagEntry");

    let newOption = new Option(tagEntry.value, tagEntry.value);
    document.getElementById("TagList").options[index++] = newOption;

    tagEntry.value = "";

    return true;
}

// Deletes the selected tag from the select list options
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

// Creates option element and adds them to the list at proper index
function ReplaceTag(tag, index) {
    let newOption = new Option(tag, tag);
    document.getElementById("TagList").options[index] = newOption;
}


$("form").on("submit", function () {
    $("#TagList option").prop("selected", "selected");
});


if (tagValues !== "") {
    let tagArray = tagValues.split(",");
    for (let i = 0; i < tagArray.length; i++) {
        ReplaceTag(tagArray[i], i);
        index++;
    }
}


