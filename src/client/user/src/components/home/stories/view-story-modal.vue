<template lang="pug">
    div.story-modal
        div.modal-content
            div.story-progress-bar
                div.story-progress-segment(
                    v-for="(s, i) in story"
                    :key="s.id"
                    :class="{ active: i === storyIndex }"
                )
            button.close-btn(@click="closeStoryModal" style="color:aliceblue") ×
            button.menu-btn(
                v-if="story && story[storyIndex].userId === useUserStore.getUserId"
                @click="showMenu = !showMenu"
                aria-label="Open menu"
            ) 
                .storySettings ...
            div.menu-modal(
                v-if="showMenu"
                @click.self="showMenu = false"
                class="mb-2"
                ref="menuModal"
                style="height: 50px"
            )
                ul.menu-list
                    li.menu-item(@click="removeStory(story[storyIndex].id); showMenu = false") Remove story
            button.prev-story(
                @click="prevStory"
            ) 
            button.next-story(
                @click="nextStory"
            ) 
            slot(name="navigation-buttons")
            img.story-image(
                v-if="story[storyIndex]?.imageUrl"
                :src="story[storyIndex].imageUrl"
                alt="story image"
            )
            a.user-info(style="cursor: pointer;text-decoration:none" @click.prevent="goToProfile(story[storyIndex].userId)") 
                img.story-circle(:src='`${gatewayUrl}users/image?userId=${story[storyIndex].userId}`' class="me-2" style='width:40px;height:40px' alt='story')
                span.story-username {{ story[storyIndex].user.fullname }} (@{{ story[storyIndex].user.username }})
</template>

<script lang="ts">
import { StoryListModel } from '@shared/models/stories/storyListModel';
import { useUserStore } from '@user/src/helpers/store';
import { PropType } from 'vue';

export default {
    name: 'ViewStoryModal',
    props: {
        selectedStory: {
            type: Array as PropType<StoryListModel[]>,
            required: true
        }
    },
    data() {
        return {
            story: [] as StoryListModel[],
            storyIndex: 0 as number,
            gatewayUrl: import.meta.env.VITE_GatewayUrl,
            useUserStore: useUserStore(),
            showMenu: false,
        }
    },
    watch: {
        selectedStory: {
            async handler(newStory) {
                this.story = [...newStory]
                this.storyIndex = 0;
                try {
                    this.setStoryImage(this.storyIndex);
                } catch (error) {
                    console.error('Error setting story image:', error);
                }
                await this.setStoryImage(this.storyIndex);
                if (!newStory[0].user) {
                    this.getUserInfo(newStory[0].userId).then(res => {
                        this.story.forEach(_ => _.user = res.data.data)
                    });
                }
            }
        }
    },
    async created() {
        Object.assign(this.story, this.selectedStory);
        this.setStoryImage(this.storyIndex)
        if (!this.story[this.storyIndex].user) {
            let user = await this.getUserInfo(this.story.userId);
            this.story.forEach(_ => _.user = user)
        }
    },
    methods: {
        async getUserInfo(userId: number) {
            var response = await this.$axios.get(`/users/${userId}`);
            return response.data.data;
        },
        async setStoryImage(index: number) {
            const imageResponse = await this.$axios.get(`/stories/image?storyId=${this.story[index].id}`, {
                responseType: 'blob',
            });
            this.story[index].imageUrl = URL.createObjectURL(imageResponse.data);
        },
        goToProfile(newUserId: number) {
            this.$router.push({ name: 'profile', query: { userId: newUserId } });
        },
        async removeStory(storyId: number) {
            var response = await this.$axios.delete(`/stories`,
                {
                    params: {
                        storyId: storyId
                    }
                }
            )
            if (!response.data.isError) {
                this.$emit('storyRemoved', storyId);
            }
        },
        closeStoryModal() {
            this.$emit('close')
        },
        prevStory() {
            if (this.storyIndex > 0) {
                this.storyIndex--;
                this.setStoryImage(this.storyIndex);
                return
            }
            this.$emit('prevStory');
        },
        nextStory() {
            if (this.storyIndex < this.story.length - 1) {
                this.storyIndex++;
                this.setStoryImage(this.storyIndex);
                return
            }
            this.$emit('nextStory')
        }
    }
}
</script>

<style scoped>
.story-modal {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: rgba(0, 0, 0, 0.7);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 1000;
}

.story-progress-bar {
    display: flex;
    position: absolute;
    top: 3px;
    left: 0;
    width: 100%;
    height: 5px;
    z-index: 20;
    padding: 0 10px;
    gap: 2px;
}

.story-progress-segment {
    flex: 1;
    background: rgba(255, 255, 255, 0.4);
    border-radius: 12px;
    transition: background 0.2s;
}

.story-progress-segment.active {
    background: rgba(255, 255, 255, 0.75);
}

.modal-content {
    background: #fff;
    border-radius: 18px;
    width: 400px;
    height: 95vh;
    box-shadow: 0 8px 32px rgba(0, 0, 0, 0.25);
    position: relative;
    display: flex;
    flex-direction: column;
    align-items: center;
}

.close-btn {
    position: absolute;
    top: 5px;
    right: 16px;
    background: none;
    border: none;
    font-size: 2rem;
    color: #888;
    cursor: pointer;
    transition: color 0.2s;
    z-index: 9999;
}

.close-btn:hover {
    color: #222;
}

.story-image {
    width: 100%;
    height: 100%;
    object-fit: cover;
    border-radius: 12px;
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.12);
    background: #f2f2f2;
}

.story-username {
    font-size: 0.8rem;
    font-weight: 600;
    color: #444444c5;
}

.user-info {
    display: flex;
    align-items: center;
    position: absolute;
    max-width: 270px;
    top: 10px;
    left: 10px;
    margin: 0 0 0.5rem 0;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.18);
    background: rgba(255, 255, 255, 0.396);
    border-radius: 8px;
    padding: 4px 10px;
}

.menu-btn {
    color: white;
    font-size: 2rem;
    font-weight: bolder;
    position: absolute;
    top: 5px;
    right: 56px;
    background: none;
    border: none;
    cursor: pointer;
    padding: 0;
    z-index: 10;
    display: flex;
    align-items: center;
    justify-content: center;
    line-height: 1;
}

.storySettings {
    flex: 0 0 50px;
    height: 50px;
}


.menu-modal {
    position: absolute;
    top: 50px;
    right: 56px;
    background: #fff;
    border-radius: 8px;
    box-shadow: 0 2px 12px rgba(0, 0, 0, 0.15);
    min-width: 120px;
    max-width: 160px;
    z-index: 20;
    padding: 0.5rem 0;
}

.menu-list {
    list-style: none;
    margin: 0;
    padding: 0;
}

.menu-item {
    padding: 0.3rem 1.2rem;
    cursor: pointer;
    color: #0c0403;
    font-size: 1rem;
    transition: background 0.2s;
}

.menu-item:hover {
    background: #f7f7f7;
}

.prev-story,
.next-story {
    position: absolute;
    background: rgba(255, 255, 255, 0.0);
    border: none;
    width: 50%;
    height: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    font-size: 1.5rem;
    color: #333;
}

.prev-story {
    left: 0px;
}

.next-story {
    right: 0px;
}

.prev-story:hover,
.next-story:hover {
    background: rgba(255, 255, 255, 0);
}
</style>