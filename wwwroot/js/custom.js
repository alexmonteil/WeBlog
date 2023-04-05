let index = 0;

// Configuration for custom sweet alert
let swalWithRedButton = Swal.mixin({
    customClass: {
        confirmButton: 'btn btn-danger btn-sm'
    },
    timer: 3000,
    buttonsStyling: false
});

// Adds the tag entered in the text box to the select list options
function AddTag() {
    let tagEntry = document.getElementById("TagEntry");
    let searchResult = SearchForErrors(tagEntry.value);

    if (searchResult != null) {

        // Alert error
        swalWithRedButton.fire({
            html: `<span class='fw-bolder'>${searchResult.toUpperCase()}</span>`,
            icon: 'error',
            confirmButtonText: 'Dismiss'
        });

    } else {

        let newOption = new Option(tagEntry.value, tagEntry.value);
        document.getElementById("TagList").options[index++] = newOption;

    }

    tagEntry.value = "";
    return true;
}

// Deletes the selected tag from the select list options
function DeleteTag() {

    let tagCount = 1;
    let tagList = document.getElementById("TagList");
    if (!tagList) {
        return false;
    }

    if (tagList.selectedIndex === -1) {
        swalWithRedButton.fire({
            html: "<span class='fw-bolder'>CHOOSE A TAG BEFORE DELETING</span>",
            icon: 'error',
            confirmButtonText: 'Dismiss'
        });

        return true;
    }

    while (tagCount > 0) {

        if (tagList.selectedIndex >= 0) {

            tagList.options[tagList.selectedIndex] = null;
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

if (tagValues !== "") {
    let tagArray = tagValues.split(",");
    for (let i = 0; i < tagArray.length; i++) {
        ReplaceTag(tagArray[i], i);
        index++;
    }
}


/* Search function detects either an empty tag or a duplicate tag on the same post
 * It returns an error string if an error is detected
 * */
function SearchForErrors(str) {
    if (str === "") {
        return "Empty tags are not permitted";
    }

    let tagElements = document.getElementById("TagList");

    if (tagElements) {

        let options = tagElements.options;
        for (let i = 0; i < options.length; i++) {
            if (options[i].value === str) {
                return `The Tag #${str} is a duplicate and cannot be used`;
            }
        }
    }
}


$("form").on("submit", function () {
    $("#TagList option").prop("selected", "selected");
});



