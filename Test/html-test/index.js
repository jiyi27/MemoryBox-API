/** @namespace data.token **/

const API_URL = 'http://localhost:5190'
let token = ''

const registerForm = document.getElementById('registerForm')
const loginForm = document.getElementById('loginForm')
const createBoxForm = document.getElementById('createBoxForm')
const createPostForm = document.getElementById('createPostForm')
const getBoxesButton = document.getElementById('getBoxesBtn')
const getPostsButton = document.getElementById('getPostsBtn')
const deletePostButton = document.getElementById('deletePostBtn')
const deleteBoxButton = document.getElementById('deleteBoxBtn')

registerForm.addEventListener('submit', async (e) => {
    // Submitting form would refresh the page, so we prevent page reload.
    e.preventDefault();

    const fullName = registerForm.regFullName.value;
    const username = registerForm.regUsername.value;
    const password = registerForm.regPassword.value;
    const email = registerForm.regEmail.value;

    const userData = {
        fullName,
        username,
        password
    };
    if (email) {
        userData.email = email;
    }

    const file = document.getElementById('profileImage').files[0];
    try {
        if (file) {
            const presignedUrlResponse = await axios.get(`${API_URL}/api/auth/presigned-url`, {
                params: { fileName: file.name, fileType: file.type }
            });
            const { presignedUrl, imageUrl } = presignedUrlResponse.data;

            console.log("Received presigned URL:", presignedUrl);

            try {
                await axios.put(presignedUrl, file, {
                    headers: {'Content-Type': file.type}
                });

                userData.profileImage = imageUrl;
            } catch (uploadError) {
                console.error("Error uploading image:", uploadError);
                return
            }
        }

        await axios.post(`${API_URL}/api/auth/register`, userData, {
            headers: {'Content-Type': 'application/json'}
        });
        console.log("User registered successfully");
    } catch (error) {
        console.error("Registration error:", error);
        handleAxiosError(error, 'register');
    }
});

loginForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    const formData = {
        username: loginForm.loginUsername.value,
        password: loginForm.loginPassword.value
    };

    try {
        const response = await axios.post(`${API_URL}/api/auth/login`, formData, {
            headers: {'Content-Type': 'application/json'}
        });

        // No need to parse JSON, axios does it for us.
        // No need to check status code, axios throws error if status code is not 2xx.
        token = data.token;
    } catch (error) {
        handleAxiosError(error, 'login');
    }
});

createBoxForm.addEventListener('submit', async (e) => {
    e.preventDefault();

    const formData = {
        boxName: createBoxForm.boxName.value,
        isPrivate: createBoxForm.isPrivate.checked
    };

    try {
        const response = await axios.post(`${API_URL}/api/boxes`, formData, {
            headers: {
                'Content-Type': 'application/json',
        }});
        console.log("Box created successfully");

    } catch (error) {
        handleAxiosError(error, 'create box');
    }
})

getBoxesButton.addEventListener('click', async () => {
    try {
        const response = await axios.get(`${API_URL}/api/boxes`);
        console.log(response.data);
    } catch (error) {
        handleAxiosError(error, 'get boxes');
    }
})

createPostForm.addEventListener('submit', async (e) => {
    e.preventDefault();

    const title = createPostForm.postTitle.value;
    const content = createPostForm.postContent.value;
    const imageFiles = document.getElementById('postImages').files;
    const boxId = document.getElementById('boxId').value;

    const formData = new FormData();
    if (title) {
        formData.append('title', title);
    }
    if (content) {
        formData.append('content', content);
    }
    for (let i = 0; i < imageFiles.length; i++) {
        formData.append('images', imageFiles[i]);
    }

    try {
        await axios.post(`${API_URL}/api/boxes/${boxId}/posts`, formData, {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        });
        createPostForm.reset();
        console.log("Post created successfully");
    } catch (error) {
        handleAxiosError(error, 'create post');
    }
});

getPostsButton.addEventListener('click', async () => {
    const boxId = document.getElementById('boxId').value;
    try {
        const response = await axios.get(`${API_URL}/api/boxes/${boxId}/posts`);
        console.log(response.data);
    } catch (error) {
        handleAxiosError(error, 'get posts');
    }
})

function handleAxiosError(error, action) {
    if (error.response) {
        console.log(`Failed to ${action}, status code:`, error.response.status);
        console.log("Error data:", error.response.data);
    } else {
        console.log(`Failed to ${action}:`, error.message);
    }
}

