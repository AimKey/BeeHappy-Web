// Upload Modal JavaScript
; (() => {
    let selectedFile = null;
    let tags = [];
    let isSubmitting = false; // Biến khóa để chống gửi 2 lần

    // DOM Elements
    const modal = document.getElementById("beetv-upload-modal");
    const closeBtn = document.getElementById("beetv-upload-close");
    const form = document.getElementById("beetv-upload-form");
    const fileInput = document.getElementById("beetv-file-input");
    const browseBtn = document.getElementById("beetv-browse-files");
    const uploadArea = document.getElementById("beetv-upload-area");
    const filePreview = document.getElementById("beetv-file-preview");
    const previewContainer = document.getElementById("beetv-preview-container");
    const fileInfo = document.getElementById("beetv-file-info");
    const removeFileBtn = document.getElementById("beetv-remove-file");
    const tagsInput = document.getElementById("beetv-emote-tags");
    const tagsContainer = document.getElementById("beetv-tags-container");
    const submitBtn = document.getElementById("beetv-upload-submit");
    const discardBtn = document.getElementById("beetv-upload-discard");
    const acceptRulesCheckbox = document.getElementById("beetv-accept-rules");
    const nameInput = document.getElementById("beetv-emote-name");

    // Lấy tham chiếu đến icon và text của nút submit
    const buttonIcon = submitBtn.querySelector("i");
    const buttonText = submitBtn.querySelector("span");

    // --- CÁC HÀM CHỨC NĂNG ---

    // Mở modal
    window.beetvOpenUploadModal = () => {
        modal.classList.remove("hidden");
        document.body.style.overflow = "hidden";
    };

    // Đóng modal
    function closeModal() {
        modal.classList.add("hidden");
        document.body.style.overflow = "";
        resetForm();
    }

    // Reset form về trạng thái ban đầu
    function resetForm() {
        form.reset();
        selectedFile = null;
        tags = [];
        tagsContainer.innerHTML = "";
        uploadArea.classList.remove("hidden");
        filePreview.classList.add("hidden");

        // Đảm bảo nút và khóa được reset
        isSubmitting = false;
        buttonIcon.classList.remove("fa-spinner", "fa-spin");
        buttonIcon.classList.add("fa-upload");
        buttonText.textContent = "Tải lên";

        updateSubmitButton();
    }

    // Xử lý file được chọn
    function handleFile(file) {
        if (!file) return;

        if (!file.type.startsWith("image/")) {
            alert("Vui lòng chọn file hình ảnh");
            return;
        }

        if (file.size > 7 * 1024 * 1024) { // 7MB
            alert("Kích thước file phải nhỏ hơn 7MB");
            return;
        }

        selectedFile = file;
        showFilePreview(file);
        updateSubmitButton();
    }

    // Hiển thị xem trước file ảnh
    function showFilePreview(file) {
        const reader = new FileReader();
        reader.onload = (e) => {
            const img = document.createElement("img");
            img.src = e.target.result;
            img.className = "max-w-32 max-h-32 rounded-lg border border-dark-border";
            previewContainer.innerHTML = "";
            previewContainer.appendChild(img);
            fileInfo.textContent = `${file.name} (${(file.size / 1024 / 1024).toFixed(2)} MB)`;
            uploadArea.classList.add("hidden");
            filePreview.classList.remove("hidden");
        };
        reader.readAsDataURL(file);
    }

    // Xử lý Thẻ (Tags)
    function addTag(tagText) {
        const trimmedTag = tagText.trim();
        if (trimmedTag && !tags.includes(trimmedTag)) {
            tags.push(trimmedTag);
            renderTags();
        }
    }

    function removeTag(tagToRemove) {
        tags = tags.filter((tag) => tag !== tagToRemove);
        renderTags();
    }

    function renderTags() {
        tagsContainer.innerHTML = "";
        tags.forEach((tag) => {
            const tagElement = document.createElement("span");
            tagElement.className = "inline-flex items-center px-3 py-1 rounded-full text-sm bg-primary/20 text-primary border border-primary/30";
            tagElement.innerHTML = `
                ${tag}
                <button type="button" class="ml-2 hover:text-primary-light" onclick="event.stopPropagation()">
                    <svg class="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path></svg>
                </button>
            `;
            tagElement.querySelector("button").addEventListener("click", () => removeTag(tag));
            tagsContainer.appendChild(tagElement);
        });
    }

    // Kiểm tra tên emote
    function validateEmoteName(name) {
        if (name.length < 1) {
            alert("Tên emote không được để trống.");
            return false;
        }
        if (name.length > 20) {
            alert("Tên emote phải có 20 ký tự hoặc ít hơn.");
            return false;
        }
        return true;
    }

    // Cập nhật trạng thái nút Submit (chỉ dựa vào tính hợp lệ của form)
    function updateSubmitButton() {
        const hasFile = selectedFile !== null;
        const hasAcceptedRules = acceptRulesCheckbox.checked;
        const hasName = nameInput.value.trim() !== "";
        submitBtn.disabled = !(hasFile && hasAcceptedRules && hasName);
    }

    // --- CÁC EVENT LISTENERS ---

    if (closeBtn) closeBtn.addEventListener("click", closeModal);
    if (browseBtn) browseBtn.addEventListener("click", () => fileInput.click());
    if (discardBtn) discardBtn.addEventListener("click", closeModal);
    if (acceptRulesCheckbox) acceptRulesCheckbox.addEventListener("change", updateSubmitButton);
    if (nameInput) nameInput.addEventListener("input", updateSubmitButton);

    if (fileInput) {
        fileInput.addEventListener("change", (e) => {
            if (e.target.files.length > 0) handleFile(e.target.files[0]);
        });
    }

    if (removeFileBtn) {
        removeFileBtn.addEventListener("click", () => {
            selectedFile = null;
            uploadArea.classList.remove("hidden");
            filePreview.classList.add("hidden");
            fileInput.value = "";
            updateSubmitButton();
        });
    }

    if (tagsInput) {
        tagsInput.addEventListener("keypress", (e) => {
            if (e.key === "Enter" || e.key === ",") {
                e.preventDefault();
                if (e.target.value.trim()) addTag(e.target.value);
                e.target.value = "";
            }
        });
        tagsInput.addEventListener("blur", (e) => {
            if (e.target.value.trim()) {
                addTag(e.target.value);
                e.target.value = "";
            }
        });
    }

    // Drag and Drop
    if (modal) {
        const dropZone = modal.querySelector("#beetv-upload-area").parentElement;
        ["dragenter", "dragover", "dragleave", "drop"].forEach(eventName => dropZone.addEventListener(eventName, preventDefaults, false));
        ["dragenter", "dragover"].forEach(eventName => dropZone.addEventListener(eventName, highlight, false));
        ["dragleave", "drop"].forEach(eventName => dropZone.addEventListener(eventName, unhighlight, false));
        dropZone.addEventListener("drop", handleDrop, false);

        function preventDefaults(e) { e.preventDefault(); e.stopPropagation(); }
        function highlight() { dropZone.classList.add("border-primary"); }
        function unhighlight() { dropZone.classList.remove("border-primary"); }

        function handleDrop(e) {
            const files = e.dataTransfer.files;
            if (files.length > 0) handleFile(files[0]);
        }
    }

    // Xử lý Submit Form (ĐÃ SỬA LỖI)
    if (form) {
        form.addEventListener("submit", async (e) => {
            e.preventDefault();

            // 1. KIỂM TRA KHÓA: Chặn click 2 lần
            if (isSubmitting) {
                return;
            }

            // 2. KHÓA SUBMIT
            isSubmitting = true;

            // 3. VALIDATION
            const emoteName = nameInput.value.trim();
            if (submitBtn.disabled || !selectedFile || !acceptRulesCheckbox.checked || !validateEmoteName(emoteName)) {
                isSubmitting = false; // Mở khóa nếu form không hợp lệ
                return;
            }

            // 4. HIỂN THỊ LOADING
            submitBtn.disabled = true;
            buttonIcon.classList.remove("fa-upload");
            buttonIcon.classList.add("fa-spinner", "fa-spin");
            buttonText.textContent = "Đang tải lên...";

            const formData = new FormData();
            formData.append("Name", emoteName);
            formData.append("Files[0].File", selectedFile);
            formData.append("Tags", tags.join(","));
            formData.append("IsOverlaying", document.getElementById("beetv-overlaying").checked);
            formData.append("Visibility", document.getElementById("beetv-private").checked ? "Private" : "Public");

            try {
                const response = await fetch("/Emote/Create", {
                    method: "POST",
                    body: formData,
                    headers: { "X-Requested-With": "XMLHttpRequest" }
                });

                if (response.ok) {
                    alert("Tải lên thành công!");
                    closeModal();
                    window.location.reload();
                } else {
                    const errorText = await response.text();
                    alert("Tải lên thất bại: " + errorText);
                }
            } catch (err) {
                console.error("Error uploading:", err);
                alert("Đã xảy ra lỗi khi tải lên.");
            } finally {
                // 5. MỞ KHÓA VÀ KHÔI PHỤC TRẠNG THÁI
                isSubmitting = false;
                buttonIcon.classList.remove("fa-spinner", "fa-spin");
                buttonIcon.classList.add("fa-upload");
                buttonText.textContent = "Tải lên";
                updateSubmitButton();
            }
        });
    }

    // Đóng modal khi click ra ngoài hoặc nhấn Escape
    if (modal) {
        modal.addEventListener("click", (e) => {
            if (e.target === modal) closeModal();
        });
    }
    document.addEventListener("keydown", (e) => {
        if (e.key === "Escape" && !modal.classList.contains("hidden")) {
            closeModal();
        }
    });
})();