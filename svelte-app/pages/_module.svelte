<script>
    import { setContext, onMount } from 'svelte';
    import { writable } from 'svelte/store';
    import { activeRoute } from "@roxi/routify";


    import util from './utils';
    import api from 'services/api';
    
    import Modal from 'svelte-simple-modal';
    import ApiDependent from 'shared/misc/ApiDependent.svelte';

    let user = writable(null); 
    setContext('user', user);

    let mainPadding;

    activeRoute.subscribe(r => {
        mainPadding = padIfNoMeta(r.meta, "_noLeftPad", "s") + padIfNoMeta(r.meta, "_noRightPad", "e") +
            padIfNoMeta(r.meta, "_noTopPad", "t") + padIfNoMeta(r.meta, "_noBottomPad", "b")
    });
    
    onMount(async () => {
        user.set(await api.get('users/me', 'Failed fetching user data'));
    });

    function padIfNoMeta(meta, metaVar, side) {
        return meta[metaVar] ? `p${side}-0 ` : `p${side}-4 `;
    }

</script>

<Modal>
    <div class="d-flex flex-column h-100">
        <nav class="top-row p-3 border-bottom navbar shadow">
            <div class="container-fluid d-flex gap-3">
                <a class="navbar-brand nav-link text-dark" href="/">MyWords</a>
                <div class="flex-grow-1"></div>

                <ApiDependent ready={$user != null}>
                    <!-- <div slot="loading"></div> -->
                    <div class="dropdown">
                        <button class="btn btn-outline-secondary mb-0" data-bs-toggle="dropdown" aria-expanded="false">
                            { ($user?.givenName ?? "") } { ($user?.familyName ?? "") }
                        </button>
                        <ul class="dropdown-menu dropdown-menu-end">
                            <li><button class="dropdown-item" on:click={() => util.navigate('/')}>Home</button></li>
                            <li><button class="dropdown-item" on:click={() => util.navigate('/collections')}>Collections</button></li>
                            <li><button class="dropdown-item" on:click={() => util.navigate('/account')}>Account</button></li>
                            <li><button class="dropdown-item" on:click={() => util.navigateBackend('/api/identity/logout')}>Logout</button></li>
                        </ul>
                    </div>
                </ApiDependent>
            </div>
        </nav>
        <!-- Main, with padding optionally disabled by route if we want stuff to go to edge. -->
        <main class={"flex-grow-1 text-start " + mainPadding}>
            <slot />
        </main>
        <!-- <div class="footer p-3">
        </div> -->
    </div>
</Modal>