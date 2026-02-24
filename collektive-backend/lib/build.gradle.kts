plugins {
    kotlin("multiplatform") version "2.2.21"
    id("it.unibo.collektive.collektive-plugin") version "27.4.0"
    id("com.squareup.wire") version "5.5.0"
}

repositories {
    mavenCentral()
}

wire {
    kotlin { }
    sourcePath {
        srcDir("src/commonMain/proto")
    }
}

kotlin {
    linuxX64("linux") {
        binaries { sharedLib { baseName = "collektive_backend" } }
    }
    mingwX64("windows") {
        binaries { sharedLib { baseName = "collektive_backend" } }
    }
    macosX64("macosX64") {
        binaries { sharedLib { baseName = "collektive_backend" } }
    }
    macosArm64("macosArm64") {
        binaries { sharedLib { baseName = "collektive_backend" } }
    }
    sourceSets {
        val commonMain by getting {
            dependencies {
                implementation("it.unibo.collektive:collektive-dsl:27.4.0")
                implementation("it.unibo.collektive:collektive-stdlib:27.4.0")
                implementation("com.squareup.wire:wire-runtime:5.5.0")
            }
        }
        val nativeMain by creating {
            dependsOn(commonMain)
        }
        val linuxMain by getting { dependsOn(nativeMain) }
        val windowsMain by getting { dependsOn(nativeMain) }
        val macosX64Main by getting { dependsOn(nativeMain) }
        val macosArm64Main by getting { dependsOn(nativeMain) }

        val commonTest by getting {
            dependencies {
                implementation(kotlin("test"))
            }
        }
        val nativeTest by creating {
            dependsOn(commonTest)
        }
        val linuxTest by getting {
            dependsOn(nativeTest)
        }
        val windowsTest by getting {
            dependsOn(nativeTest)
        }
        val macosX64Test by getting {
            dependsOn(nativeTest)
        }
        val macosArm64Test by getting {
            dependsOn(nativeTest)
        }
    }
}
