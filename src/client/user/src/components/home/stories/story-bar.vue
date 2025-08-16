<template lang="pug">
    .ig-stories-bar
        div.story-circle.ig-story.add-story(
            v-if="!stories.length || stories[0]?.userId !== useUserStore.getUserId"
            @click='addStoryModal = !addStoryModal'
            style="cursor: pointer"
        )
            span.add-icon ＋ 
        div.story-circle.ig-story(
            v-for="(story, index) in stories"
            :key="index"
            @click="openStoryModal(story)"
            style="cursor: pointer"
        )
            img(:src='`${gatewayUrl}users/image?userId=${story.userId}`' alt='story')
    ViewStoryModal(
        v-if="showModal && selectedStory"
        :selectedStory="selectedStory"
        @close="closeStoryModal"
        @storyRemoved='storyRemoved'
    )
        template(v-slot:navigation-buttons)
            button.prev-btn(@click="prevStory" :disabled="stories.findIndex(s => s.id === selectedStory?.id) === 0") ‹
            button.next-btn(@click="nextStory" :disabled="stories.findIndex(s => s.id === selectedStory?.id) === stories.length - 1") ›

    AddStoryModal(v-if='addStoryModal'  @storyCreated='storyCreated' @close='closeAddStory')
</template>

<script lang="ts">
import { ScrollModel } from '@shared/models/ScrollModel';
import { StoryListModel } from '@shared/models/stories/StoryListModel';
import { useUserStore } from '@user/src/helpers/store';
import ViewStoryModal from './view-story-modal.vue';
import AddStoryModal from './add-story-modal.vue';

export default {
    name: 'StoyBarComponent',
    components: {
        ViewStoryModal,
        AddStoryModal
    },
    data() {
        return {
            stories: [] as StoryListModel[],
            storyScrollModel: new ScrollModel(),
            gatewayUrl: import.meta.env.VITE_GatewayUrl,
            useUserStore: useUserStore(),
            showModal: false,
            selectedStory: null as StoryListModel | null,
            showMenu: false,
            addStoryModal: false
        }
    },
    async created() {
        await this.getStories();
        const story = await this.getStoryByUserId(this.useUserStore.getUserId);
        if (story) {
            this.stories.unshift(story);
        }
    },
    methods: {
        async getStories() {
            var response = await this.$axios.get(`/aggregated/stories?page=${this.storyScrollModel.currentPage}&pageSize=10`)
            console.log('Stories response:', response.data.data);
            Object.assign(this.stories, response.data.data);
        },
        async getUserInfo(userId: number) {
            var response = await this.$axios.get(`/users/${userId}`);
            return response.data.data;
        },
        async getStoryByUserId(userId: number) {
            try {
                var storyResponse = await this.$axios.get(`/stories/${userId}`);
                console.log(storyResponse.data.data);
                let user = await this.getUserInfo(userId);
                storyResponse.data.data.forEach(story => {
                    story.user = user
                });
                return storyResponse.data.data;
            } catch (error) {
                if (error.response && error.response.status === 404) {
                    console.warn('No story found for user:', userId);
                    return null;
                }
            }
        },
        prevStory() {
            const idx = this.stories.findIndex(s => s.id === this.selectedStory?.id);
            if (idx > 0) {
                this.openStoryModal(this.stories[idx - 1]);
            }
        },
        nextStory() {
            const idx = this.stories.findIndex(s => s.id === this.selectedStory?.id);
            if (idx < this.stories.length - 1) {
                this.openStoryModal(this.stories[idx + 1]);
            }
        },
        async openStoryModal(story: StoryListModel) {
            this.showMenu = false
            this.showModal = true;
            this.selectedStory ??= new StoryListModel();
            Object.assign(this.selectedStory, story);
        },
        closeStoryModal() {
            this.showModal = false;
            this.selectedStory = null;
        },
        closeAddStory() {
            this.addStoryModal = false;
        },
        storyCreated(story: StoryListModel) {
            this.stories.unshift(story);
            this.closeAddStory();
        },
        storyRemoved(storyId: number) {
            this.stories = this.stories.filter(s => s.id !== storyId);
            if (this.selectedStory?.id === storyId) {
                this.closeStoryModal();
            }
        },
    }
}
</script>

<style scoped>
.prev-btn,
.next-btn {
    display: flex;
    align-items: center;
    justify-content: center;
    position: absolute;
    top: 50%;
    transform: translateY(-50%);
    background: rgba(255, 255, 255, 0.8);
    border: none;
    font-size: 2rem;
    width: 40px;
    height: 40px;
    border-radius: 50%;
    cursor: pointer;
    z-index: 2;
    transition: background 0.2s;
}

.prev-btn {
    left: -50px;
}

.next-btn {
    right: -50px;
}

.prev-btn:disabled,
.next-btn:disabled {
    opacity: 0.4;
    cursor: not-allowed;
}

.add-story {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    background: #e0e0e0;
    color: #1976d2;
    cursor: pointer;
    border: 2px dashed #1976d2;
}

.add-icon {
    font-size: 2rem;
    font-weight: bold;
}
</style>