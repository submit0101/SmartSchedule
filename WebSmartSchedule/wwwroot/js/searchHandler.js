document.addEventListener("DOMContentLoaded", function () {
    const input = document.getElementById('searchInput');

    if (input) {
        input.addEventListener('input', performSearch);
        input.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                performSearch();
                input.blur(); 
            }
        });
    }
});

function performSearch() {
    const input = document.getElementById('searchInput');
    const filter = input.value.toLowerCase().trim();
    const previousHighlights = document.querySelectorAll('.search-highlight');
    previousHighlights.forEach(el => el.classList.remove('search-highlight'));

    if (!filter) return;
    const candidates = document.querySelectorAll('tbody tr, .card');

    let firstMatch = null;

    for (const element of candidates) {
        if (element.offsetParent === null) continue;
        const text = element.textContent.toLowerCase();
        if (text.includes(filter)) {
            firstMatch = element;
            break; 
        }
    }
    if (firstMatch) {
        firstMatch.scrollIntoView({ behavior: 'smooth', block: 'center' });
        firstMatch.classList.add('search-highlight');
    }
}