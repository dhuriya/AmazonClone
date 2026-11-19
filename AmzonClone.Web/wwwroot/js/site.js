// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", () => {
    const menuButton = document.querySelector(".mobile-menu-btn");
    const searchForm = document.querySelector(".custom-navbar .search-wrapper");
    if (menuButton && searchForm) {
        menuButton.addEventListener("click", () => {
            const isOpen = searchForm.classList.toggle("is-open");
            menuButton.setAttribute("aria-expanded", String(isOpen));
            menuButton.setAttribute("aria-label", isOpen ? "Hide product search" : "Show product search");
            if (isOpen) searchForm.querySelector("input")?.focus();
        });
    }

    document.querySelectorAll(".product-image-input").forEach(input => {
        input.addEventListener("change", () => {
            const preview = input.closest(".category-field")?.querySelector(".product-image-preview");
            const placeholder = input.closest(".category-field")?.querySelector(".product-image-preview-empty");
            const file = input.files?.[0];
            if (!preview || !placeholder) return;

            if (preview.dataset.objectUrl) URL.revokeObjectURL(preview.dataset.objectUrl);
            delete preview.dataset.objectUrl;
            if (!file) {
                const existingSource = preview.dataset.existingSrc;
                if (existingSource) {
                    preview.src = existingSource;
                    preview.hidden = false;
                    placeholder.hidden = true;
                } else {
                    preview.removeAttribute("src");
                    preview.hidden = true;
                    placeholder.hidden = false;
                }
                return;
            }
            const objectUrl = URL.createObjectURL(file);
            preview.dataset.objectUrl = objectUrl;
            preview.src = objectUrl;
            preview.hidden = false;
            placeholder.hidden = true;
        });
    });
});
