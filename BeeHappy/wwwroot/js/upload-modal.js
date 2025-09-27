// Upload Modal JavaScript
; (() => {
    let selectedFile = null
    let tags = []

    // DOM Elements
    const modal = document.getElementById("beetv-upload-modal")
    const closeBtn = document.getElementById("beetv-upload-close")
    const form = document.getElementById("beetv-upload-form")
    const fileInput = document.getElementById("beetv-file-input")
    const browseBtn = document.getElementById("beetv-browse-files")
    const uploadArea = document.getElementById("beetv-upload-area")
    const filePreview = document.getElementById("beetv-file-preview")
    const previewContainer = document.getElementById("beetv-preview-container")
    const fileInfo = document.getElementById("beetv-file-info")
    const removeFileBtn = document.getElementById("beetv-remove-file")
    const tagsInput = document.getElementById("beetv-emote-tags")
    const tagsContainer = document.getElementById("beetv-tags-container")
    const submitBtn = document.getElementById("beetv-upload-submit")
    const discardBtn = document.getElementById("beetv-upload-discard")
    const acceptRulesCheckbox = document.getElementById("beetv-accept-rules")

    // Open modal function (called from header)
    window.beetvOpenUploadModal = () => {
        modal.classList.remove("hidden")
        document.body.style.overflow = "hidden"
    }

    // Close modal function
    function closeModal() {
        modal.classList.add("hidden")
        document.body.style.overflow = ""
        resetForm()
    }

    // Reset form
    function resetForm() {
        form.reset()
        selectedFile = null
        tags = []
        tagsContainer.innerHTML = ""
        uploadArea.classList.remove("hidden")
        filePreview.classList.add("hidden")
        updateSubmitButton()
    }

    // File handling
    function handleFile(file) {
        if (!file) return

        // Validate file
        if (!file.type.startsWith("image/")) {
            alert("Please select an image file")
            return
        }

        if (file.size > 7 * 1024 * 1024) {
            // 7MB
            alert("File size must be less than 7MB")
            return
        }

        selectedFile = file
        showFilePreview(file)
        updateSubmitButton()
    }

    // Show file preview
    function showFilePreview(file) {
        const reader = new FileReader()
        reader.onload = (e) => {
            const img = document.createElement("img")
            img.src = e.target.result
            img.className = "max-w-32 max-h-32 rounded-lg border border-dark-border"

            previewContainer.innerHTML = ""
            previewContainer.appendChild(img)

            fileInfo.textContent = `${file.name} (${(file.size / 1024 / 1024).toFixed(2)} MB)`

            uploadArea.classList.add("hidden")
            filePreview.classList.remove("hidden")
        }
        reader.readAsDataURL(file)
    }

    // Tags handling
    function addTag(tagText) {
        const trimmedTag = tagText.trim()
        if (trimmedTag && !tags.includes(trimmedTag)) {
            tags.push(trimmedTag)
            renderTags()
        }
    }

    function removeTag(tagToRemove) {
        tags = tags.filter((tag) => tag !== tagToRemove)
        renderTags()
    }

    function renderTags() {
        tagsContainer.innerHTML = ""
        tags.forEach((tag) => {
            const tagElement = document.createElement("span")
            tagElement.className =
                "inline-flex items-center px-3 py-1 rounded-full text-sm bg-primary/20 text-primary border border-primary/30"
            tagElement.innerHTML = `
                ${tag}
                <button type="button" class="ml-2 hover:text-primary-light" onclick="event.stopPropagation()">
                    <svg class="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                    </svg>
                </button>
            `

            tagElement.querySelector("button").addEventListener("click", () => removeTag(tag))
            tagsContainer.appendChild(tagElement)
        })
    }

    // Update submit button state
    function updateSubmitButton() {
        const hasFile = selectedFile !== null
        const hasAcceptedRules = acceptRulesCheckbox.checked
        const hasName = document.getElementById("beetv-emote-name").value.trim() !== ""

        submitBtn.disabled = !(hasFile && hasAcceptedRules && hasName)
    }

    // Event Listeners
    if (closeBtn) {
        closeBtn.addEventListener("click", closeModal)
    }

    if (browseBtn) {
        browseBtn.addEventListener("click", () => fileInput.click())
    }

    if (fileInput) {
        fileInput.addEventListener("change", (e) => {
            if (e.target.files.length > 0) {
                handleFile(e.target.files[0])
            }
        })
    }

    if (removeFileBtn) {
        removeFileBtn.addEventListener("click", () => {
            selectedFile = null
            uploadArea.classList.remove("hidden")
            filePreview.classList.add("hidden")
            fileInput.value = ""
            updateSubmitButton()
        })
    }

    if (tagsInput) {
        tagsInput.addEventListener("keypress", (e) => {
            if (e.key === "Enter" || e.key === ",") {
                e.preventDefault()
                addTag(e.target.value)
                e.target.value = ""
            }
        })

        tagsInput.addEventListener("blur", (e) => {
            if (e.target.value.trim()) {
                addTag(e.target.value)
                e.target.value = ""
            }
        })
    }

    if (discardBtn) {
        discardBtn.addEventListener("click", closeModal)
    }

    if (acceptRulesCheckbox) {
        acceptRulesCheckbox.addEventListener("change", updateSubmitButton)
    }

    // Update submit button when name changes
    const nameInput = document.getElementById("beetv-emote-name")
    if (nameInput) {
        nameInput.addEventListener("input", updateSubmitButton)
    }

    // Drag and drop
    if (modal) {
        const dropZone = modal.querySelector("#beetv-upload-area").parentElement
            ;["dragenter", "dragover", "dragleave", "drop"].forEach((eventName) => {
                dropZone.addEventListener(eventName, preventDefaults, false)
            })

        function preventDefaults(e) {
            e.preventDefault()
            e.stopPropagation()
        }
        ;["dragenter", "dragover"].forEach((eventName) => {
            dropZone.addEventListener(eventName, highlight, false)
        })
            ;["dragleave", "drop"].forEach((eventName) => {
                dropZone.addEventListener(eventName, unhighlight, false)
            })

        function highlight(e) {
            dropZone.classList.add("border-primary")
        }

        function unhighlight(e) {
            dropZone.classList.remove("border-primary")
        }

        dropZone.addEventListener("drop", handleDrop, false)

        function handleDrop(e) {
            const dt = e.dataTransfer
            const files = dt.files
            if (files.length > 0) {
                handleFile(files[0])
            }
        }
    }

    // Form submission
    if (form) {
        form.addEventListener("submit", async (e) => {
            e.preventDefault();

            if (!selectedFile) {
                alert("Please select a file to upload");
                return;
            }

            if (!acceptRulesCheckbox.checked) {
                alert("Please accept the rules and guidelines");
                return;
            }

            const formData = new FormData();
            formData.append("Name", document.getElementById("beetv-emote-name").value);
            formData.append("Files[0].File", selectedFile);
            formData.append("Tags", tags.join(","));
            formData.append("IsOverlaying", document.getElementById("beetv-overlaying").checked);
            formData.append("IsPrivate", document.getElementById("beetv-private").checked);

            try {
                const response = await fetch("/Emote/Create", {
                    method: "POST",
                    body: formData,
                    headers: { "X-Requested-With": "XMLHttpRequest" }
                });

                if (response.ok) {
                    const result = await response.json();
                    alert("Upload successful!");
                    console.log(result);
                    closeModal();
                } else {
                    const errorText = await response.text();
                    alert("Upload failed: " + errorText);
                }
            } catch (err) {
                console.error("Error uploading:", err);
                alert("An error occurred while uploading.");
            }
        });

    }

    // Close modal when clicking outside
    if (modal) {
        modal.addEventListener("click", (e) => {
            if (e.target === modal) {
                closeModal()
            }
        })
    }

    // Close modal with Escape key
    document.addEventListener("keydown", (e) => {
        if (e.key === "Escape" && !modal.classList.contains("hidden")) {
            closeModal()
        }
    })
})()
