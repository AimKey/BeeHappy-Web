// Emote Details Page JavaScript
class EmoteDetailsPage {
    constructor() {
        this.currentPage = 1
        this.channelsPerPage = 32 // Increased for tighter grid
        this.activeTab = "channels"
        this.init()
    }

    init() {
        this.bindEvents()
        this.setupDropdowns()
        this.loadChannels()
    }

    bindEvents() {
        // Action buttons
        document.querySelectorAll("[data-action]").forEach((btn) => {
            btn.addEventListener("click", (e) => this.handleAction(e))
        })

        // Tab switching
        document.querySelectorAll("[data-tab]").forEach((tab) => {
            tab.addEventListener("click", (e) => this.switchTab(e))
        })

        // Pagination
        document.querySelectorAll("[data-direction]").forEach((btn) => {
            btn.addEventListener("click", (e) => this.handlePagination(e))
        })

        // Channel selection
        document.querySelectorAll("[data-channel]").forEach((channel) => {
            channel.addEventListener("click", (e) => this.selectChannel(e))
        })

        // Close dropdowns when clicking outside
        document.addEventListener("click", (e) => this.handleOutsideClick(e))
    }

    setupDropdowns() {
        document.querySelectorAll("[data-dropdown]").forEach((toggle) => {
            toggle.addEventListener("click", (e) => {
                e.stopPropagation()
                this.toggleDropdown(toggle.dataset.dropdown)
            })
        })
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
            case "download":
                this.downloadEmote()
                break
            case "copy-link":
                this.copyLink()
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

    switchTab(event) {
        const tabName = event.currentTarget.dataset.tab

        // Update active tab
        document.querySelectorAll(".tab").forEach((tab) => {
            tab.classList.remove("active", "bg-discord-dark", "text-white")
            tab.classList.add("text-gray-400")
        })

        event.currentTarget.classList.remove("text-gray-400")
        event.currentTarget.classList.add("active", "bg-discord-dark", "text-white")

        this.activeTab = tabName
        this.loadTabContent(tabName)
    }

    handlePagination(event) {
        const direction = event.currentTarget.dataset.direction

        if (direction === "prev" && this.currentPage > 1) {
            this.currentPage--
        } else if (direction === "next") {
            this.currentPage++
        }

        this.loadChannels()
        this.showNotification(`Page ${this.currentPage}`, "info")
    }

    selectChannel(event) {
        const channelName = event.currentTarget.dataset.channel

        // Add selection visual feedback
        document.querySelectorAll(".channel-item").forEach((item) => {
            item.classList.remove("bg-discord-blurple")
        })
        event.currentTarget.classList.add("bg-discord-blurple")

        console.log("Selected channel:", channelName)
        this.onChannelSelected(channelName)
    }

    toggleDropdown(dropdownId) {
        const dropdown = document.getElementById(dropdownId)
        const toggle = document.querySelector(`[data-dropdown="${dropdownId}"]`)

        if (dropdown && toggle) {
            const isOpen = dropdown.classList.contains("show")

            // Close all dropdowns first
            this.closeAllDropdowns()

            if (!isOpen) {
                dropdown.classList.add("show")
                toggle.closest(".dropdown").classList.add("active")
            }
        }
    }

    closeAllDropdowns() {
        document.querySelectorAll(".dropdown-menu").forEach((menu) => {
            menu.classList.remove("show")
        })
        document.querySelectorAll(".dropdown").forEach((dropdown) => {
            dropdown.classList.remove("active")
        })
    }

    handleOutsideClick(event) {
        if (!event.target.closest(".dropdown")) {
            this.closeAllDropdowns()
        }
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

    loadChannels() {
        const grid = document.querySelector(".channels-grid")
        if (grid) {
            // Add loading animation
            grid.style.opacity = "0.7"
            setTimeout(() => {
                grid.style.opacity = "1"
            }, 300)
        }

        console.log(`Loading channels page ${this.currentPage}`)
    }

    loadTabContent(tabName) {
        console.log(`Loading ${tabName} content`)

        if (tabName === "activity") {
            this.loadActivity()
        } else {
            this.loadChannels()
        }
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

    downloadEmote() {
        console.log("Downloading emote")
        this.showNotification("Download started!", "success")
    }

    copyLink() {
        console.log("Copying emote link")
        navigator.clipboard
            .writeText(window.location.href)
            .then(() => {
                this.showNotification("Link copied to clipboard!", "success")
            })
            .catch(() => {
                this.showNotification("Failed to copy link", "error")
            })
    }

    reportEmote() {
        console.log("Reporting emote")
        this.showNotification("Report submitted", "info")
    }

    onChannelSelected(channelName) {
        console.log(`Channel selected: ${channelName}`)
        this.showNotification(`Selected: ${channelName}`, "info")
    }

    loadActivity() {
        console.log("Loading activity data")
        // Add your activity loading logic here
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
    new EmoteDetailsPage()
})

// Export for use in other scripts if needed
window.EmoteDetailsPage = EmoteDetailsPage
window.EmoteUtils = EmoteUtils
