<script>
    import { getContext } from "svelte";
    import { fade } from "svelte/transition";
    import api from "services/api";

    import Loader from "shared/misc/Loader.svelte";

    const {close} = getContext('simple-modal');

    const Step = {
        PasswordInput: 0,
        AwaitingToken: 1,
        NewPasswordInput: 2,
        AwaitingSuccess: 3,
        Success: 4,
        Fail: 5,
    }

    let step = Step.PasswordInput;
    let password1 = "";
    let password2 = "";
    let token;

    async function getToken() {
        step = Step.AwaitingToken;

        let response = await api.safe.postJson('users/me/password/generatechangetoken', {password: password1}, "Failed getting password reset token");
        token = response.token;

        step = Step.NewPasswordInput;

        password1 = "";
    }

    async function sendChange() {
        step = Step.AwaitingSuccess;

        await api.safe.post('users/me/password/submitchange', {password: password1, confirmPassword: password2, token}, "Failed applying change");

        step = step.Success;
    }
</script>

<div style="height: min(250px, 50vh)" class="d-flex flex-column">
    <div class="row mb-3">
        <h4>Change password</h4>
    </div>
    
    {#if step == Step.PasswordInput || step == Step.AwaitingToken}
        <div class="d-flex">
            <p>First, enter your current password</p>
            <input class="form-control" type="password" bind:value={password1}/>
        </div>
    
        {#if password1 != ""}
            <div class="d-flex justify-content-end mt-auto" in:fade>
                {#if step == Step.AwaitingToken}
                    <Loader />
                {/if}
                <button class="btn text-secondary" on:click={getToken}>Next<i class="bi-chevron-right ms-2"></i></button>
            </div>
        {/if}
    {:else if step == Step.NewPasswordInput || step == Step.AwaitingSuccess}
        
        <div class="row">
            <p>Enter new password</p>
            <input class="form-control" type="password" bind:value={password1}/>
        </div>

        <div class="row">
            <p>Enter new password again</p>
            <input class="form-control" type="password" bind:value={password2}/>
        </div>
    
        <!-- todo: chekcing stuff -->

        {#if password1 != "" && password2 != "" && password1 == password2}
            <div class="d-flex justify-content-end mt-auto" in:fade>
                {#if step == Step.AwaitingSuccess}
                    <Loader />
                {/if}
                <button class="btn text-secondary" on:click={sendChange}>Next<i class="bi-chevron-right ms-2"></i></button>
            </div>
        {/if}
    {:else if step == Step.Success}
        <div>
            Password updated successfully
        </div>
        <div class="d-flex mt-3">
            <button class="btn btn-secondary" on:click={close}>Ok</button>
        </div>
    {/if}
</div>