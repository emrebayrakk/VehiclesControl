// Global keyboard shortcut: Ctrl/⌘ + K focuses the search box in the top bar.
document.addEventListener("keydown", (event) => {
    if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === "k") {
        const input = document.querySelector("#global-search input");
        if (input) {
            event.preventDefault();
            input.focus();
            input.select();
        }
    }
});
