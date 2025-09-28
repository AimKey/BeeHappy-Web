// Edit Emote Modal JavaScript
class EditEmoteModal {
    constructor() {
        this.modal = document.getElementById("editModal")
        this.form = document.getElementById("editEmoteForm")
        this.tagInput = document.getElementById("tagInput")
        this.tagsList = document.getElementById("tagsList")
        this.tagsHidden = document.getElementById("tagsHidden")

        this.init()
    }

    init() {
        this.bindEvents()
        this.updateTagsHiddenInput()
    }

    bindEvents() {
        // Close modal events
        const closeBtn = document.getElementById("closeModalBtn")
        const cancelBtn = document.getElementById("cancelBtn")

        if (closeBtn) closeBtn.addEventListener("click", () => this.hide())
        if (cancelBtn) cancelBtn.addEventListener("click", () => this.hide())

        // Close on backdrop click
        if (this.modal) {
            this.modal.addEventListener("click", (e) => {
                if (e.target === this.modal) {
                    this.hide()
                }
            })
        }

        // Tag input handling
        if (this.tagInput) {
            // Prevent spaces from being typed
            this.tagInput.addEventListener("keydown", (e) => {
                if (e.key === " ") {
                    e.preventDefault()
                    return false
                }
            })
            
            // Only allow Enter key to add tags
            this.tagInput.addEventListener("keypress", (e) => {
                if (e.key === "Enter") {
                    e.preventDefault()
                    this.addTag()
                }
            })
        }

        // Remove tag buttons
        this.bindRemoveTagEvents()

        // Toggle switches
        this.bindToggleEvents()

        // Delete button
        const deleteBtn = document.getElementById("deleteEmoteBtn")
        if (deleteBtn) {
            deleteBtn.addEventListener("click", () => this.confirmDelete())
        }

        // Form submission
        if (this.form) {
            this.form.addEventListener("submit", (e) => {
                e.preventDefault()
                this.submitEditForm()
            })
        }
    }

    show() {
        if (this.modal) {
            this.modal.classList.remove("hidden")
            this.modal.classList.add("show")
            document.body.style.overflow = "hidden"
        }
    }

    hide() {
        if (this.modal) {
            this.modal.classList.add("hidden")
            this.modal.classList.remove("show")
            document.body.style.overflow = "auto"
        }
    }

    addTag() {
        const tagText = this.tagInput.value.trim()

        if (tagText && tagText.length > 0) {
            // Check if tag already exists
            const existingTags = this.getCurrentTags()
            if (existingTags.includes(tagText)) {
                this.tagInput.value = ""
                return
            }

            this.addTagToList(tagText)
            this.tagInput.value = ""
            this.updateTagsHiddenInput()
        }
    }

    addTagToList(tagText) {
        const tagElement = document.createElement("span")
        tagElement.className =
            "tag-item bg-dark-surface/60 text-white text-sm px-3 py-1 rounded-full flex items-center space-x-2 border border-dark-border"
        tagElement.innerHTML = `
            <span>${tagText}</span>
            <button type="button" class="remove-tag text-gray-400 hover:text-white">
                <i class="fas fa-times text-xs"></i>
            </button>
        `

        this.tagsList.appendChild(tagElement)
        this.bindRemoveTagEvents()
    }

    bindRemoveTagEvents() {
        const removeButtons = document.querySelectorAll(".remove-tag")
        removeButtons.forEach((btn) => {
            btn.removeEventListener("click", this.handleRemoveTag) // Remove existing listeners
            btn.addEventListener("click", this.handleRemoveTag.bind(this))
        })
    }

    handleRemoveTag(e) {
        e.target.closest(".tag-item").remove()
        this.updateTagsHiddenInput()
    }

    getCurrentTags() {
        const tagElements = this.tagsList.querySelectorAll(".tag-item span:first-child")
        return Array.from(tagElements).map((span) => span.textContent.trim())
    }

    updateTagsHiddenInput() {
        const tags = this.getCurrentTags()
        if (this.tagsHidden) {
            this.tagsHidden.value = tags.join(",")
        }
    }

    bindToggleEvents() {
        const toggles = document.querySelectorAll(".toggle-switch")
        toggles.forEach((toggle) => {
            toggle.addEventListener("click", () => {
                const targetId = toggle.dataset.toggle
                const checkbox = document.getElementById(targetId)

                if (checkbox) {
                    checkbox.checked = !checkbox.checked
                    toggle.classList.toggle("active", checkbox.checked)
                }
            })
        })
    }

    updateVisibilityHiddenInput() {
        const privateToggle = document.getElementById("privateToggle")
        const visibilityInput = document.querySelector('input[name="Visibility"]')

        if (privateToggle && visibilityInput) {
            visibilityInput.value = privateToggle.checked ? "Private" : "Public"
        }
    }

    updateOverlayingHiddenInput() {
        const overlayingToggle = document.getElementById("overlayingToggle")
        const overlayingInput = document.querySelector('input[name="IsOverlaying"]')

        if (overlayingToggle && overlayingInput) {
            overlayingInput.value = overlayingToggle.checked ? "true" : "false"
        }
    }

    confirmDelete() {
        if (confirm("Are you sure you want to delete this emote? This action cannot be undone.")) {
            const deleteForm = document.getElementById("deleteForm")
            if (deleteForm) {
                deleteForm.submit()
            }
        }
    }

    submitEditForm() {
        console.log("[v0] Submitting edit form")

        // Update hidden inputs before submission
        this.updateTagsHiddenInput()
        this.updateVisibilityHiddenInput()
        this.updateOverlayingHiddenInput()

        // Get form data
        const formData = new FormData(this.form)

        // Submit form via fetch or traditional form submission
        this.form.submit()
    }
}

// Initialize modal when DOM is loaded
document.addEventListener("DOMContentLoaded", () => {
    window.editEmoteModal = new EditEmoteModal()
})

// Export for use in other scripts
window.EditEmoteModal = EditEmoteModal
