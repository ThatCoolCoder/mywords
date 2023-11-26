<!-- routify:options preload="proximity" -->

<script>
    import { setContext, onMount } from 'svelte';
    import api from '../services/api';
    import ErrorPopup from '../shared/errorPopup.svelte';
    
    function goToAccount() {
        history.pushState({}, null, '/account');
    }

    var errorPopup;

    var user = null;
    setContext('user', {
        get: () => user,
        set: newUser => user = newUser
    });

    async function fetchAccountInfo() {
        const response = await fetch('api/users/me', {
            credentials: 'include'
        });
        user = await response.json();
        
    }
    
    onMount(fetchAccountInfo);
    onMount(() => api.onError = errorPopup.open);

</script>



<div class="d-flex flex-column h-100">
    <nav class="top-row p-3 border-bottom navbar shadow">
        <div class="container-fluid d-flex gap-3">
            <a class="navbar-brand nav-link text-dark" href="/">MyWords</a>
            <div class="flex-grow-1"></div>
            <!-- <a class="nav-link text-dark" href="login">Login</a>
            <a class="nav-link text-dark" href="signup">Sign Up</a> -->
            <!-- <button on:click={goToAccount}>Account</button> -->

            <div class="dropdown">
                <button class="btn" data-bs-toggle="dropdown" aria-expanded="false">
                  { (user?.givenName ?? "") } { (user?.familyName ?? "") }
                </button>
                <ul class="dropdown-menu dropdown-menu-end">
                  <li><button class="dropdown-item" href="#" on:click={goToAccount}>My Account</button></li>
                </ul>
              </div>
        </div>
    </nav>
    <main class="flex-shrink-0 px-4 py-4">
        <slot />
    </main>
    <ErrorPopup bind:this={errorPopup} />
    <!-- <div class="footer p-3">
    </div> -->
</div>