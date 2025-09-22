// Emotes Platform JavaScript functionality
document.addEventListener("DOMContentLoaded", () => {
    // Modal functionality
    const signInBtn = document.getElementById("signInBtn")
    const loginModal = document.getElementById("loginModal")
    const closeModal = document.getElementById("closeModal")

    // Open modal
    signInBtn.addEventListener("click", () => {
        loginModal.classList.remove("hidden")
        loginModal.classList.add("flex")
        document.body.style.overflow = "hidden"

        // Add fade-in animation
        setTimeout(() => {
            loginModal.querySelector(".bg-dark-surface").classList.add("animate-fade-in-up")
        }, 10)
    })

    // Close modal
    function closeLoginModal() {
        loginModal.classList.add("hidden")
        loginModal.classList.remove("flex")
        document.body.style.overflow = "auto"
        loginModal.querySelector(".bg-dark-surface").classList.remove("animate-fade-in-up")
    }

    closeModal.addEventListener("click", closeLoginModal)

    // Close modal when clicking outside
    loginModal.addEventListener("click", (e) => {
        if (e.target === loginModal) {
            closeLoginModal()
        }
    })

    // Close modal with Escape key
    document.addEventListener("keydown", (e) => {
        if (e.key === "Escape" && !loginModal.classList.contains("hidden")) {
            closeLoginModal()
        }
    })

    // Social login handlers
    const discordBtn = loginModal.querySelector(".bg-\\[\\#5865F2\\]")
    const twitchBtn = loginModal.querySelector(".bg-\\[\\#9146FF\\]")

    discordBtn.addEventListener("click", () => {
        handleSocialLogin("discord")
    })

    twitchBtn.addEventListener("click", () => {
        handleSocialLogin("twitch")
    })

    function handleSocialLogin(platform) {
        // Add loading state
        const button = event.target.closest("button")
        button.classList.add("loading")

        // Simulate API call (replace with actual authentication logic)
        setTimeout(() => {
            console.log(`Authenticating with ${platform}...`)

            // In a real application, you would:
            // 1. Redirect to OAuth provider
            // 2. Handle the callback
            // 3. Store authentication tokens
            // 4. Redirect to dashboard

            // For demo purposes, just show an alert
            alert(`${platform.charAt(0).toUpperCase() + platform.slice(1)} authentication would be handled here`)

            button.classList.remove("loading")
            closeLoginModal()
        }, 1500)
    }

    // Smooth scrolling for anchor links
    document.querySelectorAll('a[href^="#"]').forEach((anchor) => {
        anchor.addEventListener("click", function (e) {
            e.preventDefault()
            const target = document.querySelector(this.getAttribute("href"))
            if (target) {
                target.scrollIntoView({
                    behavior: "smooth",
                    block: "start",
                })
            }
        })
    })

    // Add scroll-based animations
    const observerOptions = {
        threshold: 0.1,
        rootMargin: "0px 0px -50px 0px",
    }

    const observer = new IntersectionObserver((entries) => {
        entries.forEach((entry) => {
            if (entry.isIntersecting) {
                entry.target.classList.add("animate-fade-in-up")
            }
        })
    }, observerOptions)

    // Observe feature cards
    document.querySelectorAll(".bg-dark-surface").forEach((card) => {
        observer.observe(card)
    })

    // Mobile menu toggle (if needed)
    const mobileMenuBtn = document.querySelector(".md\\:hidden button")
    if (mobileMenuBtn) {
        mobileMenuBtn.addEventListener("click", () => {
            // Toggle mobile menu (implement as needed)
            console.log("Mobile menu toggle")
        })
    }

    // Search functionality
    const searchInput = document.querySelector('input[placeholder="Search..."]')
    if (searchInput) {
        searchInput.addEventListener("input", (e) => {
            const query = e.target.value.toLowerCase()
            // Implement search functionality as needed
            console.log("Search query:", query)
        })
    }

    // Add floating animation to the 3D decoration
    const decoration = document.querySelector('img[alt="3D Crystal Decoration"]')
    if (decoration) {
        decoration.classList.add("animate-float")
    }

    // Handle form submissions (if any forms are added later)
    document.addEventListener("submit", (e) => {
        const form = e.target
        if (form.tagName === "FORM") {
            e.preventDefault()

            // Add loading state to submit button
            const submitBtn = form.querySelector('button[type="submit"]')
            if (submitBtn) {
                submitBtn.classList.add("loading")

                // Simulate form submission
                setTimeout(() => {
                    submitBtn.classList.remove("loading")
                    // Handle success/error states
                }, 2000)
            }
        }
    })

    // Performance optimization: Lazy load images
    if ("IntersectionObserver" in window) {
        const imageObserver = new IntersectionObserver((entries, observer) => {
            entries.forEach((entry) => {
                if (entry.isIntersecting) {
                    const img = entry.target
                    img.src = img.dataset.src
                    img.classList.remove("lazy")
                    imageObserver.unobserve(img)
                }
            })
        })

        document.querySelectorAll("img[data-src]").forEach((img) => {
            imageObserver.observe(img)
        })
    }

    // Add keyboard navigation support
    document.addEventListener("keydown", (e) => {
        // Tab navigation improvements
        if (e.key === "Tab") {
            document.body.classList.add("keyboard-navigation")
        }
    })

    document.addEventListener("mousedown", () => {
        document.body.classList.remove("keyboard-navigation")
    })

    // Console welcome message
    console.log("%c🐝 Welcome to BeeTV!", "color: #8B5CF6; font-size: 24px; font-weight: bold;")
    console.log("%cThe Emote Platform For All", "color: #06B6D4; font-size: 16px;")
})

// Utility functions
function debounce(func, wait) {
    let timeout
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout)
            func(...args)
        }
        clearTimeout(timeout)
        timeout = setTimeout(later, wait)
    }
}

function throttle(func, limit) {
    let inThrottle
    return function () {
        const args = arguments

        if (!inThrottle) {
            func.apply(this, args)
            inThrottle = true
            setTimeout(() => (inThrottle = false), limit)
        }
    }
}

// Export functions for potential use in other scripts
window.EmotesPlatform = {
    debounce,
    throttle,
    closeLoginModal: () => {
        const loginModal = document.getElementById("loginModal")
        loginModal.classList.add("hidden")
        loginModal.classList.remove("flex")
        document.body.style.overflow = "auto"
    },
}
