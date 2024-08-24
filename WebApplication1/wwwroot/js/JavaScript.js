function confirmDelete(uniqueId, isDeleteClicked) {
   
    var deleteSpan = "deleteSpan_" + uniqueId;
    var confirmSpan = "confirmDeleteSpan_" + uniqueId;

    deleteSpanBlock = document.getElementById(deleteSpan);
    confirmDeleteSpanBlock = document.getElementById(confirmSpan);
    console.log(deleteSpanBlock);

    if (isDeleteClicked) {
        deleteSpanBlock.style.display = "none";
        confirmDeleteSpanBlock.style.display = "inline";
    }
    else {
        deleteSpanBlock.style.display = "inline";
        confirmDeleteSpanBlock.style.display = "none";
    }
}