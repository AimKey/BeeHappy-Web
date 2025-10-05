// User Dropdown Management
// Scoped to avoid conflicts with other components
; (() => {
    // User dropdown functionality
    function initUserDropdown() {
        const userDropdownBtn = document.getElementById("beetv-user-button")
        const userDropdownMenu = document.getElementById("beetv-user-dropdown-menu")
        const userDropdownArrow = document.getElementById("beetv-dropdown-arrow")

        if (!userDropdownBtn || !userDropdownMenu) {
            return // Elements not found, user probably not logged in
        }

        // Toggle dropdown on button click
        userDropdownBtn.addEventListener("click", (e) => {
            e.stopPropagation()

            const isOpen = !userDropdownMenu.classList.contains("opacity-0")

            if (isOpen) {
                closeDropdown()
            } else {
                openDropdown()
            }
        })

        // Close dropdown when clicking outside
        document.addEventListener("click", (e) => {
            if (!userDropdownBtn.contains(e.target) && !userDropdownMenu.contains(e.target)) {
                closeDropdown()
            }
        })

        // Close dropdown on escape key
        document.addEventListener("keydown", (e) => {
            if (e.key === "Escape") {
                closeDropdown()
            }
        })

        function openDropdown() {
            userDropdownMenu.classList.remove("opacity-0", "invisible", "translate-y-2")
            userDropdownBtn.setAttribute("aria-expanded", "true")
            if (userDropdownArrow) {
                userDropdownArrow.style.transform = "rotate(180deg)"
            }
        }

        function closeDropdown() {
            userDropdownMenu.classList.add("opacity-0", "invisible", "translate-y-2")
            userDropdownBtn.setAttribute("aria-expanded", "false")
            if (userDropdownArrow) {
                userDropdownArrow.style.transform = "rotate(0deg)"
            }
        }
    }

    // Sign out functionality
    window.beetvSignOut = () => {
        if (confirm("Are you sure you want to sign out?")) {
            // You can customize this URL based on your logout route
            window.location.href = "/Logout/Logout"
        }
    }

    // Initialize when DOM is ready
    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initUserDropdown)
    } else {
        initUserDropdown()
    }
})()
