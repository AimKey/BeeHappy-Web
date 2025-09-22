class EmotesDirectory {
  constructor() {
    this.currentView = "grid"
    this.init()
  }

  init() {
    this.bindEvents()
  }

  bindEvents() {
    // Search form submission is now handled by the HTML form

    // View controls
    const viewBtns = document.querySelectorAll(".view-btn[data-view]")
    viewBtns.forEach((btn) => {
      btn.addEventListener("click", () => {
        // Remove active class from all buttons
        viewBtns.forEach((b) => b.classList.remove("active"))
        // Add active class to clicked button
        btn.classList.add("active")

        this.currentView = btn.dataset.view
        this.updateViewMode()
      })
    })

    const searchForm = document.getElementById("searchForm")
    if (searchForm) {
      searchForm.addEventListener("submit", (e) => {
        // Let the form submit naturally to the server
        // No need to prevent default since we want server-side handling
      })
    }

    const searchInput = document.getElementById("searchInput")
    const tagsInput = document.getElementById("tagsInput")

    if (searchInput) {
      searchInput.addEventListener("keypress", (e) => {
        if (e.key === "Enter") {
          document.getElementById("searchForm").submit()
        }
      })
    }

    if (tagsInput) {
      tagsInput.addEventListener("keypress", (e) => {
        if (e.key === "Enter") {
          document.getElementById("searchForm").submit()
        }
      })
    }

    this.bindEmoteCardEvents()
  }

  bindEmoteCardEvents() {
    const existingCards = document.querySelectorAll(".emote-card")
    existingCards.forEach((card) => {
      card.addEventListener("click", () => {
        const emoteId = card.dataset.emoteId
        const emoteName = card.querySelector(".emote-name")?.textContent
        const emoteOwner = card.querySelector(".emote-owner")?.textContent
        const emoteImg = card.querySelector("img")?.src

        this.showEmoteDetails({
          id: emoteId,
          name: emoteName,
          owner: emoteOwner,
          thumbnail: emoteImg,
        })
      })
    })
  }

  showEmoteDetails(emote) {
    // TODO: Implement emote details modal
    console.log("Show emote details:", emote)
  }

  updateViewMode() {
    const grid = document.getElementById("emotesGrid")
    if (!grid) return

    if (this.currentView === "list") {
      grid.classList.add("list-view")
    } else {
      grid.classList.remove("list-view")
    }
  }
}

// Initialize when DOM is loaded
document.addEventListener("DOMContentLoaded", () => {
  new EmotesDirectory()
})
