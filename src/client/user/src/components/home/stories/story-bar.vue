<template lang="pug">
    .ig-stories-bar
        div.story-circle.ig-story.add-story(
            v-if="!allStories.length || allStories[0][0]?.userId !== useUserStore.getUserId"
            @click='addStoryModal = !addStoryModal'
            style="cursor: pointer"
        )
            span.add-icon ＋ 
        div.story-circle.ig-story(
            v-for="(story, index) in allStories"
            :key="index"
            @click="openStoryModal(story, index)"
            style="cursor: pointer"
        )
            img(:src='`${gatewayUrl}users/image?userId=${story[0].userId}`' alt='story')
    ViewStoryModal(
        v-if="showModal && selectedStory"
        :selectedStory="selectedStory"
        @close="closeStoryModal"
        @storyRemoved='storyRemoved'
        @prevStory='prevStory'
        @nextStory='nextStory'
    )
        template(v-slot:navigation-buttons)
            button.prev-btn(@click="prevStory" :disabled="selectedStoryIndex <= 0") ‹
            button.next-btn(@click="nextStory" :disabled="selectedStoryIndex >= allStories.length - 1") ›

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
            allStories: [] as StoryListModel[][],
            storyScrollModel: new ScrollModel(),
            gatewayUrl: import.meta.env.VITE_GatewayUrl,
            useUserStore: useUserStore(),
            showModal: false,
            selectedStory: [] as StoryListModel[],
            selectedStoryIndex: 0,
            showMenu: false,
            addStoryModal: false
        }
    },
    async created() {
        await this.getAllStories();
        const story = await this.getStoryByUserId(this.useUserStore.getUserId);
        if (story) {
            this.allStories.unshift(story);
        }
    },
    methods: {
        async getAllStories() {
            var response = await this.$axios.get(`/aggregated/stories?page=${this.storyScrollModel.currentPage}&pageSize=10`)
            Object.assign(this.allStories, response.data.data);
        },
        async getUserInfo(userId: number) {
            var response = await this.$axios.get(`/users/${userId}`);
            return response.data.data;
        },
        async getStoryByUserId(userId: number) {
            try {
                var storyResponse = await this.$axios.get(`/stories/${userId}`);
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
            this.selectedStoryIndex--;
            if (this.selectedStoryIndex < 0) {
                this.closeStoryModal();
                return;
            }
            this.openStoryModal(this.allStories[this.selectedStoryIndex], this.selectedStoryIndex);
        },
        nextStory() {
            this.selectedStoryIndex++;
            if (this.selectedStoryIndex > this.allStories.length - 1) {
                this.closeStoryModal();
                return;
            }
            this.openStoryModal(this.allStories[this.selectedStoryIndex], this.selectedStoryIndex);
        },
        async openStoryModal(story: StoryListModel[], index: number) {
            this.showMenu = false
            this.showModal = true;
            this.selectedStoryIndex = index;
            this.selectedStory = [] as StoryListModel[];
            Object.assign(this.selectedStory, story);
        },
        closeStoryModal() {
            this.showModal = false;
            this.selectedStoryIndex = 0;
            this.selectedStory = null;
        },
        closeAddStory() {
            this.addStoryModal = false;
        },
        storyCreated(story: StoryListModel) {
            let userStory = this.allStories.find(s => s[0].userId === story.userId)
            if (!userStory) {
                this.allStories.unshift([story]);
            } else {
                userStory.unshift(story);
            }
            this.closeAddStory();
        },
        storyRemoved(storyId: number) {
            this.allStories = this.allStories.filter(s => {
                let res = s.filter(story => story.id !== storyId)
                return res.length !== 0
            })
            if (this.selectedStory[0]?.id === storyId) {
                this.closeStoryModal();
            }

            console.log('Story removed:', this.allStories[0][0]?.userId);
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