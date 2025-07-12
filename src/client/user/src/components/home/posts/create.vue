<template>
    <div class="mt-5">
        <button @click="openModal">Create New Post</button>
        <div v-if="showModal" class="modal-overlay">
            <div class="modal">
                <h2>Create Post</h2>
                <form @submit.prevent="submitPost">
                    <div>
                        <label for="title">Title:</label>
                        <input id="title" v-model="title" required />
                    </div>
                    <div>
                        <label for="content">Content:</label>
                        <textarea id="content" v-model="content" required></textarea>
                    </div>
                    <div class="modal-actions">
                        <button type="submit">Post</button>
                        <button type="button" @click="closeModal">Cancel</button>
                    </div>
                </form>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    data() {
        return {
            showModal: false,
            title: '',
            content: ''
        };
    },
    methods: {
        openModal() {
            this.showModal = true;
        },
        closeModal() {
            this.showModal = false;
            this.title = '';
            this.content = '';
        },
        submitPost() {
            const post = {
                title: this.title,
                content: this.content,
                createdAt: new Date()
            };
            alert('Post created:\n' + JSON.stringify(post, null, 2));
            this.closeModal();
        }
    }
};
</script>

<style scoped>
.modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: rgba(0, 0, 0, 0.5);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 99999;
    /* Increased z-index */
}

.modal {
    background: #fff;
    padding: 2rem;
    border-radius: 8px;
    min-width: 300px;
    z-index: 100000;
    /* Increased z-index */
    box-shadow: 0 2px 16px rgba(0, 0, 0, 0.3);
    /* Add shadow for visibility */
}

.modal-actions {
    margin-top: 1rem;
    display: flex;
    gap: 1rem;
    justify-content: flex-end;
}
</style>