module MemoryManagement
    using .Native
    export sharpref
    const jl_mem_lock = ReentrantLock()
    const jl_freed_references = Array{Int32}(undef, 0)
    const jl_references = Array{Any}(undef, 0)
    
    function _MallocJuliaObject(v::Any)
        lock(jl_mem_lock)
        if length(jl_freed_references) != 0
            ref = pop!(jl_freed_references)
            jl_references[ref] = v
            return ref
        else
            push!(jl_references, v)
            return length(jl_references)
        end
        unlock(jl_mem_lock)
    end

    function _FreeJuliaObject(ptr::Int32)
        lock(jl_mem_lock)

        jl_references[ptr] = nothing

        if ptr == length(jl_references)
            deleteat!(jl_references, ptr)
        else
            push!(jl_freed_references, ptr)
        end

        unlock(jl_mem_lock)
    end

    _DereferenceJuliaObject(ptr::Int32) = (lock(jl_mem_lock); jl_references[ptr]; unlock(jl_mem_lock))

    function _init()
        @sharpfunction(_MallocSharpObject, (v::NativeObject), v)
        @sharpfunction(_FreeSharpObject, (v::Int32), NativeObject(Ptr{Cvoid}(v)))
        @sharpfunction(_DereferenceSharpObject, (v::Int32), NativeObject(Ptr{Cvoid}(v)))
        @sharpfunction(_CreateSafeSharpJuliaReference, (x::Any), NativeObject(Native.unsaferef(x)))
    end
end
