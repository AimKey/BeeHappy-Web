// Simplified Emote Details Page JavaScript for Index Page
console.log("[v0] Emote Details script loaded successfully!")

class EmoteDetailsPage {
    constructor() {
        console.log("[v0] EmoteDetailsPage constructor called")
        this.init()
    }

    init() {
        console.log("[v0] Initializing EmoteDetailsPage")
        this.bindEvents()
        this.setupDropdowns()
        this.setupModalEvents()
    }

    bindEvents() {
        // Action buttons
        document.querySelectorAll("[data-action]").forEach((btn) => {
            btn.addEventListener("click", (e) => this.handleAction(e))
        })

        // Close dropdowns when clicking outside
        document.addEventListener("click", (e) => this.handleOutsideClick(e))
    }

    setupDropdowns() {
        console.log("[v0] Setting up dropdowns...")
        const dropdownToggles = document.querySelectorAll("[data-dropdown]")
        console.log("[v0] Found dropdown toggles:", dropdownToggles.length)

        dropdownToggles.forEach((toggle) => {
            console.log("[v0] Found dropdown toggle:", toggle.dataset.dropdown)
            toggle.addEventListener("click", (e) => {
                console.log("[v0] Dropdown clicked:", toggle.dataset.dropdown)
                e.preventDefault()
                e.stopPropagation()
                this.toggleDropdown(toggle.dataset.dropdown)
            })
        })
    }

    toggleDropdown(dropdownId) {
        console.log("[v0] Toggling dropdown:", dropdownId)
        const dropdown = document.getElementById(dropdownId)
        const toggle = document.querySelector(`[data-dropdown="${dropdownId}"]`)

        console.log("[v0] Dropdown element:", dropdown)
        console.log("[v0] Toggle element:", toggle)

        if (dropdown && toggle) {
            const isOpen = dropdown.classList.contains("show")
            console.log("[v0] Dropdown is currently open:", isOpen)

            // Close all dropdowns first
            this.closeAllDropdowns()

            if (!isOpen) {
                console.log("[v0] Opening dropdown")
                dropdown.classList.add("show")
                toggle.closest(".dropdown").classList.add("active")
            }
        } else {
            console.log("[v0] Dropdown or toggle not found!")
        }
    }

    closeAllDropdowns() {
        console.log("[v0] Closing all dropdowns")
        document.querySelectorAll(".dropdown-menu").forEach((menu) => {
            menu.classList.remove("show")
        })
        document.querySelectorAll(".dropdown").forEach((dropdown) => {
            dropdown.classList.remove("active")
        })
    }

    handleOutsideClick(event) {
        console.log("[v0] Outside click detected")
        if (!event.target.closest(".dropdown")) {
            console.log("[v0] Click outside dropdown, closing all")
            this.closeAllDropdowns()
        }
    }

    handleAction(event) {
        const action = event.currentTarget.dataset.action
        const button = event.currentTarget

        // Add loading state
        this.setLoading(button, true)

        switch (action) {
            case "add-to-anime":
                this.addToAnime()
                break
            case "add-to-collection":
                this.addToCollection()
                break
            case "edit":
                this.editEmote()
                break
            case "download":
                this.downloadEmote()
                break
            case "copy-image":
                this.copyImage()
                break
            case "report":
                this.reportEmote()
                break
            default:
                console.log("Unknown action:", action)
        }

        // Remove loading state after delay
        setTimeout(() => this.setLoading(button, false), 1000)
    }

    setLoading(element, isLoading) {
        if (isLoading) {
            element.classList.add("opacity-50", "pointer-events-none")
            element.disabled = true
        } else {
            element.classList.remove("opacity-50", "pointer-events-none")
            element.disabled = false
        }
    }

    downloadEmote() {
        console.log("Downloading emote")

        // Get all emote images from the page
        const emoteImages = document.querySelectorAll('img[src*="@"]')

        if (emoteImages.length === 0) {
            // Fallback: get any image in the emote variants section
            const variantImages = document.querySelectorAll(".bg-discord-dark img")
            if (variantImages.length > 0) {
                const lastImage = variantImages[variantImages.length - 1]
                this.downloadImage(lastImage.src, "emote")
            } else {
                this.showNotification("No emote image found to download", "error")
            }
            return
        }

        // If multiple variants, download the first one (or you could show a selection)
        if (emoteImages.length === 1) {
            this.downloadImage(emoteImages[0].src, "emote")
        } else {
            // Download all variants
            emoteImages.forEach((img, index) => {
                setTimeout(() => {
                    this.downloadImage(img.src, `emote_variant_${index + 1}`)
                }, index * 500) // Stagger downloads
            })
            this.showNotification(`Downloading ${emoteImages.length} emote variants...`, "success")
            return
        }

        this.showNotification("Download started!", "success")
    }

    downloadImage(imageUrl, filename) {
        // Create a temporary anchor element
        const link = document.createElement("a")
        link.href = imageUrl
        link.download = filename || "emote"

        // Handle cross-origin images by fetching and creating blob URL
        fetch(imageUrl)
            .then((response) => response.blob())
            .then((blob) => {
                const blobUrl = window.URL.createObjectURL(blob)
                link.href = blobUrl
                document.body.appendChild(link)
                link.click()
                document.body.removeChild(link)
                window.URL.revokeObjectURL(blobUrl)
            })
            .catch((error) => {
                console.error("Download failed:", error)
                // Fallback: try direct download
                document.body.appendChild(link)
                link.click()
                document.body.removeChild(link)
            })
    }

    copyImage() {
        console.log("Copying emote image")

        // Get the first emote image
        const emoteImage = document.querySelector(".bg-discord-dark img")

        if (!emoteImage) {
            this.showNotification("No emote image found to copy", "error")
            return
        }

        // Create a canvas to convert image to blob
        const canvas = document.createElement("canvas")
        const ctx = canvas.getContext("2d")

        // Create a new image element to ensure it's loaded
        const img = new Image()
        img.crossOrigin = "anonymous" // Handle CORS

        img.onload = () => {
            canvas.width = img.width
            canvas.height = img.height
            ctx.drawImage(img, 0, 0)

            canvas.toBlob(async (blob) => {
                try {
                    await navigator.clipboard.write([
                        new ClipboardItem({
                            "image/png": blob,
                        }),
                    ])
                    this.showNotification("Image copied to clipboard!", "success")
                } catch (error) {
                    console.error("Failed to copy image:", error)
                    // Fallback: copy image URL
                    this.copyImageUrl(emoteImage.src)
                }
            }, "image/png")
        }

        img.onerror = () => {
            console.error("Failed to load image for copying")
            // Fallback: copy image URL
            this.copyImageUrl(emoteImage.src)
        }

        img.src = emoteImage.src
    }

    copyImageUrl(imageUrl) {
        navigator.clipboard
            .writeText(imageUrl)
            .then(() => {
                this.showNotification("Image URL copied to clipboard!", "success")
            })
            .catch(() => {
                this.showNotification("Failed to copy image", "error")
            })
    }

    // Action Methods
    addToAnime() {
        console.log("Adding emote to anime collection")
        this.showNotification("Emote added to anime collection!", "success")
    }

    addToCollection() {
        console.log("Adding emote to collection")
        this.showNotification("Choose a collection to add to", "info")
    }

    editEmote() {
        console.log("[v0] Opening edit emote modal")
        if (window.editEmoteModal) {
            window.editEmoteModal.show()
        } else {
            console.error("[v0] EditEmoteModal not found")
            this.showNotification("Modal not available", "error")
        }
    }

    reportEmote() {
        console.log("Reporting emote")
        this.showNotification("Report submitted", "info")
        // Add actual report logic here
    }

    // Modal functionality methods
    showEditModal() {
        const modal = document.getElementById("editEmoteModal")
        if (modal) {
            // Populate modal with current emote data
            this.populateModalData()
            modal.classList.remove("hidden")
            modal.classList.add("flex")
            document.body.style.overflow = "hidden"
        }
    }

    hideEditModal() {
        const modal = document.getElementById("editEmoteModal")
        if (modal) {
            modal.classList.add("hidden")
            modal.classList.remove("flex")
            document.body.style.overflow = "auto"
        }
    }

    populateModalData() {
        // Get current emote data from the page
        const emoteName = document.querySelector(".text-2xl.font-bold")?.textContent || ""
        const tags = Array.from(document.querySelectorAll(".bg-discord-dark.text-discord-text")).map((tag) =>
            tag.textContent.trim(),
        )

        // Populate form fields
        const nameInput = document.getElementById("emoteName")
        if (nameInput) nameInput.value = emoteName

        // Clear and populate tags
        const tagsContainer = document.getElementById("currentTags")
        if (tagsContainer) {
            tagsContainer.innerHTML = ""
            tags.forEach((tag) => this.addTagToModal(tag))
        }
    }

    setupModalEvents() {
        // Close modal events
        const closeButtons = document.querySelectorAll("[data-modal-close]")
        closeButtons.forEach((btn) => {
            btn.addEventListener("click", () => this.hideEditModal())
        })

        // Close on backdrop click
        const modal = document.getElementById("editEmoteModal")
        if (modal) {
            modal.addEventListener("click", (e) => {
                if (e.target === modal) {
                    this.hideEditModal()
                }
            })
        }

        // Tag input handling
        const tagInput = document.getElementById("tagInput")
        if (tagInput) {
            tagInput.addEventListener("keypress", (e) => {
                if (e.key === "Enter") {
                    e.preventDefault()
                    this.addTag()
                }
            })
        }

        // Form submission
        const saveBtn = document.getElementById("saveEmote")
        if (saveBtn) {
            saveBtn.addEventListener("click", () => this.saveEmoteChanges())
        }

        // Delete button
        const deleteBtn = document.getElementById("deleteEmote")
        if (deleteBtn) {
            deleteBtn.addEventListener("click", () => this.deleteEmote())
        }

        // Toggle switches
        const overlayingToggle = document.getElementById("overlayingToggle")
        const privateToggle = document.getElementById("privateToggle")

        if (overlayingToggle) {
            overlayingToggle.addEventListener("change", () => this.handleToggleChange("overlaying", overlayingToggle.checked))
        }

        if (privateToggle) {
            privateToggle.addEventListener("change", () => this.handleToggleChange("private", privateToggle.checked))
        }
    }

    addTag() {
        const tagInput = document.getElementById("tagInput")
        const tagText = tagInput.value.trim()

        if (tagText && tagText.length > 0) {
            this.addTagToModal(tagText)
            tagInput.value = ""
        }
    }

    addTagToModal(tagText) {
        const tagsContainer = document.getElementById("currentTags")
        if (!tagsContainer) return

        // Check if tag already exists
        const existingTags = Array.from(tagsContainer.children).map((tag) => tag.textContent.replace("×", "").trim())
        if (existingTags.includes(tagText)) return

        const tagElement = document.createElement("span")
        tagElement.className = "inline-flex items-center gap-1 px-2 py-1 bg-discord-dark text-discord-text rounded text-sm"
        tagElement.innerHTML = `
            ${tagText}
            <button type="button" class="text-discord-muted hover:text-discord-red transition-colors" onclick="this.parentElement.remove()">×</button>
        `

        tagsContainer.appendChild(tagElement)
    }

    handleToggleChange(setting, isEnabled) {
        console.log(`${setting} toggle changed to:`, isEnabled)
        // Store the setting change for when form is submitted
        if (!this.pendingChanges) this.pendingChanges = {}
        this.pendingChanges[setting] = isEnabled
    }

    saveEmoteChanges() {
        const nameInput = document.getElementById("emoteName")
        const tagsContainer = document.getElementById("currentTags")

        const formData = {
            name: nameInput?.value || "",
            tags: Array.from(tagsContainer?.children || []).map((tag) => tag.textContent.replace("×", "").trim()),
            settings: this.pendingChanges || {},
        }

        console.log("Saving emote changes:", formData)

        // Add loading state
        const saveBtn = document.getElementById("saveEmote")
        this.setLoading(saveBtn, true)

        // Simulate API call
        setTimeout(() => {
            this.setLoading(saveBtn, false)
            this.hideEditModal()
            this.showNotification("Emote updated successfully!", "success")

            // Update the page with new data
            this.updatePageData(formData)
        }, 1000)
    }

    updatePageData(formData) {
        // Update emote name on page
        const nameElement = document.querySelector(".text-2xl.font-bold")
        if (nameElement && formData.name) {
            nameElement.textContent = formData.name
        }

        // Update tags on page
        const tagsContainer = document.querySelector(".flex.flex-wrap.gap-2")
        if (tagsContainer && formData.tags) {
            // Clear existing tags
            tagsContainer.innerHTML = ""

            // Add new tags
            formData.tags.forEach((tag) => {
                const tagElement = document.createElement("span")
                tagElement.className = "px-2 py-1 bg-discord-dark text-discord-text rounded text-sm"
                tagElement.textContent = tag
                tagsContainer.appendChild(tagElement)
            })
        }
    }

    deleteEmote() {
        if (confirm("Are you sure you want to delete this emote? This action cannot be undone.")) {
            console.log("Deleting emote...")
            this.showNotification("Emote deleted successfully!", "success")

            // Simulate redirect after deletion
            setTimeout(() => {
                window.location.href = "/emotes"
            }, 1500)
        }
    }

    showNotification(message, type = "info") {
        const notification = document.createElement("div")
        notification.className = "notification fixed top-5 right-5 px-4 py-2 rounded-lg text-white font-medium z-50 text-sm"
        notification.textContent = message

        // Set background color based on type
        const typeClasses = {
            success: "bg-discord-green",
            error: "bg-discord-red",
            info: "bg-discord-blurple",
            warning: "bg-discord-yellow text-black",
        }
        notification.classList.add(typeClasses[type] || typeClasses.info)

        document.body.appendChild(notification)

        // Remove after delay
        setTimeout(() => {
            notification.style.opacity = "0"
            notification.style.transform = "translateY(-20px)"
            setTimeout(() => {
                if (notification.parentNode) {
                    notification.parentNode.removeChild(notification)
                }
            }, 300)
        }, 2500)
    }
}

// Utility Functions
const EmoteUtils = {
    formatFileSize(bytes) {
        const sizes = ["Bytes", "KB", "MB", "GB"]
        if (bytes === 0) return "0 Bytes"
        const i = Math.floor(Math.log(bytes) / Math.log(1024))
        return Math.round((bytes / Math.pow(1024, i)) * 100) / 100 + " " + sizes[i]
    },

    formatDate(date) {
        return new Intl.DateTimeFormat("vi-VN", {
            year: "numeric",
            month: "short",
            day: "numeric",
        }).format(new Date(date))
    },

    debounce(func, wait) {
        let timeout
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout)
                func(...args)
            }
            clearTimeout(timeout)
            timeout = setTimeout(later, wait)
        }
    },
}

// Initialize when DOM is loaded
document.addEventListener("DOMContentLoaded", () => {
    console.log("[v0] DOM loaded, initializing EmoteDetailsPage")
    new EmoteDetailsPage()
})

// Export for use in other scripts if needed
window.EmoteDetailsPage = EmoteDetailsPage
window.EmoteUtils = EmoteUtils
